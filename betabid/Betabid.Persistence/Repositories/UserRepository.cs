using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DataContext dbContext) : base(dbContext)
    {
    }
}