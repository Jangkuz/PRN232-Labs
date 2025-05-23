using BusinessObject;
using PRN232.Lab1.Repo.UnitOfWork;
using PRN232.Lab1.Service.Interfaces;

namespace PRN232.Lab1.Service.Implement;

public class StoreAccountService : IStoreAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public StoreAccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StoreAccount?> GetAccountByIdAsync(int accountId)
    {
        return await _unitOfWork.StoreAccountRepository.GetByIdAsync(accountId);
    }
}
