using Betabid.Application.Interfaces.Repositories;
using Betabid.Domain.Entities;
using Betabid.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Betabid.Persistence.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    private readonly DataContext _dataContext;
    
    public CommentRepository(DataContext dbContext)
        : base(dbContext)
    {
        _dataContext = dbContext;
    }

    public new async Task<IEnumerable<Comment>> GetAllAsync()
    {
        return await _dataContext.Comments
            .Include(c => c.ParentComment)
            .Include(c => c.User)
            .ToListAsync();
    }
}