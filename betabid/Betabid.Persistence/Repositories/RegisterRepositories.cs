using Betabid.Application.Interfaces.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Betabid.Persistence.Repositories;

public static class RegisterRepositories
{
    public static void AddScopedRepositories(this IServiceCollection builder)
    {
        builder.AddScoped<IBetRepository, BetRepository>();
        builder.AddScoped<ILotRepository, LotRepository>();
        builder.AddScoped<IPictureRepository, PictureRepository>();
        builder.AddScoped<ISavedRepository, SavedRepository>();
        builder.AddScoped<IUserRepository, UserRepository>();
        builder.AddScoped<ITagRepository, TagRepository>();
        builder.AddScoped<IUnitOfWork, UnitOfWork>();
    }
    
}