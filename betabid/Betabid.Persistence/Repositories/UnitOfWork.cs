using Betabid.Application.Interfaces.Repositories;
using Betabid.Persistence.Context;

namespace Betabid.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private bool _disposed;
    
    public UnitOfWork(
        DataContext context,
        ITagRepository tags,
        IBetRepository bets,
        ILotRepository lots,
        IPictureRepository pictures,
        ISavedRepository saved,
        IUserRepository users,
        ICommentRepository comments)
    {
        _context = context;
        Tags = tags;
        Bets = bets;
        Lots = lots;
        Pictures = pictures;
        Saved = saved;
        Users = users;
        Comments = comments;
    }

    public IBetRepository Bets { get; }
    
    public ILotRepository Lots { get; }
    
    public IPictureRepository Pictures { get; }
    
    public ISavedRepository Saved { get; }
    
    public IUserRepository Users { get; }
    
    public ITagRepository Tags { get; set; }
    
    public ICommentRepository Comments { get; set; }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }
}