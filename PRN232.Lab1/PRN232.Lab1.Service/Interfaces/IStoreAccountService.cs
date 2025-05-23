using BusinessObject;

namespace PRN232.Lab1.Service.Interfaces;

public interface IStoreAccountService
{
    Task<StoreAccount?> GetAccountByIdAsync(int accountId);
}
