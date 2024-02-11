using Betabid.Application.DTOs.BetDtos;
using Betabid.Application.DTOs.FilteringDto;
using Betabid.Application.DTOs.UserDtos;
using Betabid.Application.Services.Interfaces;
using betabid.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace betabid.Controllers;

[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto loginDto)
    {
        var result = await _userService.LoginUserAsync(loginDto);

        return Ok(result);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        await _userService.RegisterUserAsync(registerUserDto);

        return Ok();
    }
    
    [HttpGet]
    [Route("{userId}")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var userDto = await _userService.GetUserByIdAsync(userId);

        return Ok(userDto);
    }

    [HttpPut]
    [Authorize]
    [Route("update-user-data")]
    public async Task<IActionResult> UpdateUserData([FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await _userService.UpdateUserDataAsync(updateUserDto);

        return Ok();
    }
    
    [HttpPut]
    [Authorize]
    [Route("update-user-password")]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UpdateUserPasswordDto updateUserDto)
    {
        await _userService.UpdateUserPasswordAsync(updateUserDto);

        return Ok();
    }
    
    [HttpDelete]
    [Authorize]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteUserById([FromRoute] string id)
    {
        await _userService.DeleteUserByIdAsync(id);

        return Ok();
    }
    
    [HttpPost]
    [Authorize]
    [Route("save")]
    public async Task<IActionResult> SaveLot([FromBody] SaveLotRequestDto saveLotRequestDto)
    {
        await _userService.SaveLotAsync(saveLotRequestDto);

        return Ok();
    }
    
    [HttpGet]
    [Route("user-lots")]
    public async Task<IActionResult> GetUserLotsAsync([FromQuery] FilteringOptionsDto filteringOptionsDto)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        
        var lots = await _userService.GetUserLotsAsync(userId, filteringOptionsDto);

        return Ok(lots);
    }
    
    [HttpPost]
    [Route("bet")]
    public async Task<IActionResult> BetAsync([FromBody] MakeBetDto makeBetDto)
    {
        var userId = await this.GetUserIdFromJwtAsync();
        if (userId is null)
        {
            return Unauthorized();
        }
        
        await _userService.BetAsync(userId, makeBetDto);

        return Ok();
    }
}