using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(DataContext dbContext) : base(dbContext)
    {
    }
}