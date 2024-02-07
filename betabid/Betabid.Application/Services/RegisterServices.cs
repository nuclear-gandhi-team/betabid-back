using Betabid.Application.Services.Implementations;
using Betabid.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Betabid.Application.Services;

public static class RegisterServices
{
    public static void AddScopedServices(this IServiceCollection builder)
    {
        builder.AddScoped<ILotService, LotService>();
    }
}