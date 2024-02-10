using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace betabid.Extensions;

public static class JwtParserExtensions
{
    public static async Task<string?> GetUserIdFromJwtAsync(this ControllerBase controllerBase)
    {
        var authenticateResult = await controllerBase.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

        controllerBase.HttpContext.User = authenticateResult.Principal!;

        if (authenticateResult.Succeeded)
        {
            var userPrincipal = authenticateResult.Principal;
            if (userPrincipal.Identity is not null && userPrincipal.Identity.IsAuthenticated)
            {
                var userId = userPrincipal.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                    .Value
                    ?? throw new UnauthorizedAccessException("User Id is null. Error authorizing.");

                return userId;
            }
        }

        return null!;
    }
}