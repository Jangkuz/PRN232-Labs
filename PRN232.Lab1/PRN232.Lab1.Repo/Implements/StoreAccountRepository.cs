using BusinessObject;
using PRN232.Lab1.Repo.Interfaces;

namespace PRN232.Lab1.Repo.Implements;

public class StoreAccountRepository : GenericRepository<StoreAccount, int>, IStoreAccountRepository
{
    public StoreAccountRepository(Lab1PharmaceuticalDbContext context) : base(context)
    {
    }
}
