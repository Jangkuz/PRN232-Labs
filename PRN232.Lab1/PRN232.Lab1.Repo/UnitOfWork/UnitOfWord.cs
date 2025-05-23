using BusinessObject;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PRN232.Lab1.Repo.Interfaces;

namespace PRN232.Lab1.Repo.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly Lab1PharmaceuticalDbContext _context;
    private IDbContextTransaction _transaction = null!;
    private IServiceProvider _serviceProvider;

    private Dictionary<Type, object> _repositories = new();

    public IManufactureRepository ManufactureRepository { get; private set; }
    public IMedicineInfomationReposiroty MedicineInfomationReposiroty { get; private set; }
    public IStoreAccountRepository StoreAccountRepository { get; private set; }


    public UnitOfWork(
        Lab1PharmaceuticalDbContext context,
        IServiceProvider serviceProvider,
        IManufactureRepository manufacture,
        IMedicineInfomationReposiroty medicine,
        IStoreAccountRepository storeAccount
        )
    {
        _context = context;
        _serviceProvider = serviceProvider;

        ManufactureRepository = manufacture;
        MedicineInfomationReposiroty = medicine;
        StoreAccountRepository = storeAccount;
        //ManufactureRepository = new ManufactureRepository(_context);
        //MedicineInfomationReposiroty = new MedicineInfomationRepository(_context);
        //StoreAccountRepository = new StoreAccountRepository(_context);

    }

    public IGenericRepository<TEntity, TEntityId> GetRepo<TEntity, TEntityId>()
        where TEntity : BaseEntity<TEntityId>
        where TEntityId : notnull
    {
        return _serviceProvider.GetRequiredService<IGenericRepository<TEntity, TEntityId>>();
    }

    //public IGenericRepository<TEntity, TEntityId> GetRepo<TEntity, TEntityId>()
    //    where TEntity : BaseEntity<TEntityId>
    //    where TEntityId : notnull
    //{
    //    if (!_repositories.ContainsKey(typeof(TEntity)))
    //    {
    //        var repo = new GenericRepository<TEntity, TEntityId>(_context);
    //        _repositories.Add(typeof(TEntity), repo);
    //    }
    //    return (IGenericRepository<TEntity, TEntityId>)_repositories[typeof(TEntity)];
    //}
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null!;
        }
    }

    private bool disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            disposed = true;
        }
    }


    public async Task RollBackAsync()
    {
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null!;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
