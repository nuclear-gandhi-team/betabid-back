using Betabid.Application.DTOs.UserDtos;

namespace Betabid.Application.Interfaces.Services;

public interface IUserService
{
    Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto);
    
    Task RegisterUserAsync(RegisterUserDto registerUserDto);

    Task<GetFullUserDto> GetUserByIdAsync(string id);

    Task DeleteUserByIdAsync(string id);

    Task UpdateUserDataAsync(UpdateUserDto updateUserDto);

    Task UpdateUserPasswordAsync(UpdateUserPasswordDto updateUserDto);

    Task SaveLotAsync(SaveLotRequestDto saveLotRequestDto);
}