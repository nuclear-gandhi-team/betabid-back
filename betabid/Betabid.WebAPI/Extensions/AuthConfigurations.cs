using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace betabid.Extensions;

public static class AuthConfigurations
{
    public static void AddAuthConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"]
                            ?? throw new NullReferenceException("JWT Secret was not set"))),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["JWT:Issuer"]
                                      ?? throw new NullReferenceException("JWT Issuer is not set"),
                        ValidateAudience = true,
                        ValidAudience = configuration["JWT:Audience"],
                    };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsync("Unauthorized.");
                    },
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Authentication failed. Exception details: " + context.Exception);
                        if (context.Request.Headers.ContainsKey("Authorization"))
                        {
                            Console.WriteLine("Authorization header: " + context.Request.Headers["Authorization"]);
                        }
                        else
                        {
                            Console.WriteLine("No Authorization header present.");
                        }

                        return Task.CompletedTask;
                    },
                };
            });
    }
}
