using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Betabid.Application.DTOs.BetDtos;
using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.DTOs.UserDtos;
using Betabid.Application.Exceptions;
using Betabid.Application.Filtering;
using Betabid.Application.Filtering.Lots;
using Betabid.Application.Helpers;
using Betabid.Application.Helpers.Options;
using Betabid.Application.Interfaces.Repositories;
using Betabid.Application.Services.Interfaces;
using Betabid.Domain.Entities;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
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
    private readonly ITimeProvider _timeProvider;

    private readonly JwtOptions _jwtOptions;
    
    public UserService(
        UserManager<User> userManager,
        IMapper mapper,
        IOptions<JwtOptions> jwtOptions,
        IUnitOfWork unitOfWork,
        ITimeProvider timeProvider)
    {
        _userManager = userManager;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
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

    [Authorize]
    public async Task<GetFullUserDto> GetUserByIdAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        return _mapper.Map<GetFullUserDto>(user)!;
    }

    [Authorize]
    public async Task DeleteUserByIdAsync(string id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id)
                   ?? throw new EntityNotFoundException($"No user with Id '{id}'");

        await _userManager.DeleteAsync(user);
    }

    [Authorize]
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

    [Authorize]
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

    [Authorize]
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

    public async Task<LotsWithPagination> GetUserLotsAsync(string userId, FilteringOptionsDto filteringOptionsDto)
    {
        if (await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId) is null)
        {
            throw new EntityNotFoundException($"No user with Id '{userId}'");
        }
                   
        var predicate = PredicateBuilder.New<Lot>(true);
        var specifications = new List<IFilteringCriteria<Lot>>();
        
        specifications.Add(new LotOwnerFilteringCriteria(userId));
        
        if (filteringOptionsDto.Tags != null && filteringOptionsDto.Tags.Any())
        {
            specifications.Add(new TagsFilteringCriteria(filteringOptionsDto.Tags));
        }
        if (filteringOptionsDto.Status != null)
        {
            specifications.Add(new LotStatusFilteringCriteria(filteringOptionsDto.Status.Value, _timeProvider));
        }
        if (filteringOptionsDto.NameStartsWith != null)
        {
            specifications.Add(new LotNameFilteringCriteria(filteringOptionsDto.NameStartsWith));
        }
        
        predicate = specifications.Aggregate(predicate, (current, specification) => current.And(specification.Criteria));

        var (filteredLots, totalCount) = await _unitOfWork.Lots.GetAllFilteredAsync(predicate, filteringOptionsDto);

        if (!filteredLots.Any())
        {
            return new LotsWithPagination();
        }
    
        var lotsDto = _mapper.Map<IEnumerable<GetLotCardDto>>(filteredLots)!.ToList();

        foreach (var lotDto in lotsDto)
        {
            var lot = filteredLots.FirstOrDefault(lot => lot.Id == lotDto.Id);
            if (lot == null)
            {
                continue;
            }

            if (userId is not null)
            {
                lotDto.IsSaved = await _unitOfWork.Lots.IsLotSavedByUserAsync(lotDto.Id, userId);
            }
            else
            {
                lotDto.IsSaved = false;
            }

            lotDto.Status = new StatusHelper(_timeProvider).GetStatus(lot); 

            var tags = await _unitOfWork.Lots.GetByIdWithTagsAsync(lotDto.Id);
            lotDto.Tags = tags?.Tags?.Select(t => t.Name).ToList() ?? new List<string> { "Other" };

            var picture = await _unitOfWork.Pictures.GetPictureByLotIdAsync(lotDto.Id);
            lotDto.Image = picture != null ? Convert.ToBase64String(picture.Data) : null;
        }
    
        return new LotsWithPagination
        {
            Lots = lotsDto,
            TotalPages = totalCount,
            CurrentPage = filteringOptionsDto.Page ?? 1
        };
    }

    public async Task BetAsync(string userId, MakeBetDto makeBetDto)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new EntityNotFoundException($"No user with Id '{userId}'");

        if (user.Balance < makeBetDto.Amount)
        {
            throw new InsufficientFundsException($"Not enough money to bet ${makeBetDto.Amount}");
        }
        
        var lot = await _unitOfWork.Lots.GetByIdWithBetsAsync(makeBetDto.LotId)
            ?? throw new EntityNotFoundException($"No lot with Id '{makeBetDto.LotId}'");

        if (makeBetDto.Amount < lot.BetStep)
        {
            throw new InvalidBetStepException($"Your bet must be at least {lot.BetStep} greater than last bet.");
        }

        if (lot.DateStarted > _timeProvider.Now)
        {
            throw new LotDateException("Bids for this lot is not started yet.");
        }
        if(lot.Deadline < _timeProvider.Now)
        {
            throw new LotDateException("Bids for this lot already closed.");
        }

        lot.Bets.Add(new Bet
        {
            Amount = makeBetDto.Amount,
            Time = _timeProvider.Now,
            UserId = userId,
            LotId = makeBetDto.LotId,
        });

        user.Balance -= makeBetDto.Amount;

        if (lot.Bets.Count >= 2)
        {
            lot.Bets[^2].User.Balance += lot.Bets[^2].Amount;
        }

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