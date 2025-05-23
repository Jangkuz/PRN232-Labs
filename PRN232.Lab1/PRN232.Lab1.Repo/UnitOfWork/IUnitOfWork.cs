using BusinessObject;
using PRN232.Lab1.Repo.Interfaces;

namespace PRN232.Lab1.Repo.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IManufactureRepository ManufactureRepository { get; }
    IMedicineInfomationReposiroty MedicineInfomationReposiroty { get; }
    IStoreAccountRepository StoreAccountRepository { get; }

    IGenericRepository<TEntity, TEntityId> GetRepo<TEntity, TEntityId>()
        where TEntity : BaseEntity<TEntityId>
        where TEntityId : notnull;
    Task SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollBackAsync();
    Task<bool> SaveAsync();
}
