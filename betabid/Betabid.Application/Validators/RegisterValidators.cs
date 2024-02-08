using Betabid.Application.DTOs.LotsDTOs;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Betabid.Application.Validators;

public static class RegisterValidators
{
    public static void AddValidators(this IServiceCollection builder)
    {
        builder.AddTransient<IValidator<AddLotDto>, AddLotValidator>();
    }
}