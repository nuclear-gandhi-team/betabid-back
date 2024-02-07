namespace Betabid.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    IBetRepository Bets { get; }
    
    ILotRepository Lots { get; }
    
    IPictureRepository Pictures { get; }
    
    ISavedRepository Saved { get; }
    
    IUserRepository Users { get; }
    
    ITagRepository Tags { get; }
    
    Task CommitAsync();
}