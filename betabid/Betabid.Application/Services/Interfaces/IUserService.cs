using Betabid.Application.DTOs.BetDtos;
using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.LotsDTOs;
using Betabid.Application.DTOs.UserDtos;

namespace Betabid.Application.Services.Interfaces;

public interface IUserService
{
    Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto);
    
    Task RegisterUserAsync(RegisterUserDto registerUserDto);

    Task<GetFullUserDto> GetUserByIdAsync(string id);

    Task DeleteUserByIdAsync(string id);

    Task UpdateUserDataAsync(UpdateUserDto updateUserDto);

    Task UpdateUserPasswordAsync(UpdateUserPasswordDto updateUserDto);

    Task SaveLotAsync(SaveLotRequestDto saveLotRequestDto);
    
    Task<LotsWithPagination> GetUserLotsAsync(string userId, FilteringOptionsDto filteringOptionsDto);

    Task BetAsync(string userId, MakeBetDto makeBetDto);
}