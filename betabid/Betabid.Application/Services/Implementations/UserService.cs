using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Betabid.Application.DTOs.UserDtos;
using Betabid.Application.Exceptions;
using Betabid.Application.Helpers.Options;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Application.Interfaces.Services;
using Betabid.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Betabid.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private readonly JwtOptions _jwtOptions;
    
    public UserService(UserManager<User> userManager, IMapper mapper, IOptions<JwtOptions> jwtOptions, IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _jwtOptions = jwtOptions.Value;
    }
    
    public async Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto)
    {
        var userExists = await _userManager.FindByNameAsync(loginUserDto.Login)
                         ?? throw new ArgumentException("User doest exists");

        return await _userManager.CheckPasswordAsync(userExists, loginUserDto.Password)
            ? new LoginResponseDto { Token =  GenerateJwtAsync(userExists) }
            : throw new AuthenticationException("Wrong password");
    }

    public async Task RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var userExists = await _userManager.Users.FirstOrDefaultAsync(u => u.Name == registerUserDto.Login);

        if (userExists is not null)
        {
            throw new ArgumentException("User with this Name already exists.");
        }

        var newUser = _mapper.Map<User>(registerUserDto) ?? throw new ArgumentException("Invalid data.");

        var result = await _userManager.CreateAsync(newUser, registerUserDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Aggregate(string.Empty, (current, err) => current + err.Description);

            throw new ArgumentException($"User cannot be created: {errors}");
        }
    }

    public async Task<GetFullUserDto> GetUserByIdAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        return _mapper.Map<GetFullUserDto>(user)!;
    }

    public async Task DeleteUserByIdAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new EntityNotFoundException($"No user with Id '{id}'");

        await _userManager.DeleteAsync(user);
    }

    public async Task UpdateUserDataAsync(UpdateUserDto updateUserDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == updateUserDto.Id)
                   ?? throw new EntityNotFoundException($"No user with Id '{updateUserDto.Id}'");

        if (updateUserDto.NewName is not null)
        {
            user.Name = updateUserDto.NewName;
            user.UserName = updateUserDto.NewName;
            user.NormalizedUserName = updateUserDto.NewName.ToUpperInvariant();
        }

        if (updateUserDto.NewEmail is not null)
        {
            user.Email = updateUserDto.NewEmail;
            user.NormalizedUserName = updateUserDto.NewEmail.ToUpperInvariant();
        }

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new UserUpdateException($"Error updating user with Id '{user.Id}'");
        }
    }

    public async Task UpdateUserPasswordAsync(UpdateUserPasswordDto updateUserDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == updateUserDto.Id)
                   ?? throw new EntityNotFoundException($"No user with Id '{updateUserDto.Id}'");

        if (!await _userManager.CheckPasswordAsync(user, updateUserDto.OldPassword))
        {
            throw new UserUpdateException("Wrong password.");
        }

        if (updateUserDto.NewPassword != updateUserDto.ConfirmNewPassword)
        {
            throw new UserUpdateException("Passwords are not equal.");
        }
        
        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
        {
            throw new UserUpdateException($"Error deleting password for user Id '{user.Id}'");
        }

        var addPasswordResult = await _userManager.AddPasswordAsync(user, updateUserDto.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            throw new UserUpdateException($"Error adding new password for user Id '{user.Id}'");
        }
    }

    public async Task SaveLotAsync(SaveLotRequestDto saveLotRequestDto)
    {
        var user = await _userManager.Users
                       .FirstOrDefaultAsync(u => u.Id == saveLotRequestDto.UserId)
                   ?? throw new EntityNotFoundException($"No user with Id '{saveLotRequestDto.UserId}'");

        var lot = await _unitOfWork.Lots.GetByIdAsync(saveLotRequestDto.LotId)
            ?? throw new EntityNotFoundException($"No lot with Id {saveLotRequestDto.LotId}");
        
        await _unitOfWork.Saved.AddAsync(new Saved { User = user, Lot = lot });

        await _unitOfWork.CommitAsync();
    }

    private string GenerateJwtAsync(User user)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Secret));

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: DateTime.UtcNow.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return $"Bearer {tokenValue}";
    }

}