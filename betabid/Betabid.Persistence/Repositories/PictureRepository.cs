using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class PictureRepository : Repository<Picture>, IPictureRepository
{
    public PictureRepository(DataContext dbContext) : base(dbContext)
    {
    }
}