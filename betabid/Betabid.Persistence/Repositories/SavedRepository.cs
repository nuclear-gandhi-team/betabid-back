using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class SavedRepository : Repository<Saved>, ISavedRepository
{
    public SavedRepository(DataContext dbContext) : base(dbContext)
    {
    }
}