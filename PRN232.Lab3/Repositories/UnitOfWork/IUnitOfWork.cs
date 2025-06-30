using DataAccessLayer.Interfaces;
using Repositories.Entities;

namespace Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    //IManufactureRepository ManufactureRepository { get; }

    IGenericRepository<TEntity, TEntityId> GetRepo<TEntity, TEntityId>()
        where TEntity : BaseEntity<TEntityId>
        where TEntityId : notnull;
    Task SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollBackAsync();
    Task<bool> SaveAsync();
}
