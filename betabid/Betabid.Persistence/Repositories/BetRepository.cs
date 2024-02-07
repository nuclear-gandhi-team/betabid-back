using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class BetRepository : Repository<Bet>, IBetRepository
{
    public BetRepository(DataContext dbContext) : base(dbContext)
    {
    }
}