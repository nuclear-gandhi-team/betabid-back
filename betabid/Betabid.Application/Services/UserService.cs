using AutoMapper;
using Betabid.Domain.Entities;
using Betabid.Features.Common.Options;
using Betabid.Features.Exceptions;
using Betabid.Features.Interfaces;
using Betabid.Features.UserFeatures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Betabid.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private readonly JwtOptions _jwtOptions;
    
    public UserService(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        IMapper mapper,
        IOptions<JwtOptions> jwtOptions)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _jwtOptions = jwtOptions.Value;
    }
    
    public Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto)
    {
        throw new NotImplementedException();
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
        }

        if (updateUserDto.NewEmail is not null)
        {
            user.Name = updateUserDto.NewEmail;
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
}