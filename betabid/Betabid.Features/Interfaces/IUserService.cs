using Betabid.Features.UserFeatures;

namespace Betabid.Features.Interfaces;

public interface IUserService
{
    Task<LoginResponseDto> LoginUserAsync(LoginUserDto loginUserDto);
    
    Task RegisterUserAsync(RegisterUserDto registerUserDto);

    Task<GetFullUserDto> GetUserByIdAsync(string id);

    Task DeleteUserByIdAsync(string id);

    Task UpdateUserDataAsync(UpdateUserDto updateUserDto);

    Task UpdateUserPasswordAsync(UpdateUserPasswordDto updateUserDto);
}