using Betabid.Features.Interfaces;
using Betabid.Features.UserFeatures;
using Microsoft.AspNetCore.Mvc;

namespace betabid.Controllers;

[Route("[controller]")]
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

}