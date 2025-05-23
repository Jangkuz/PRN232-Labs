using BusinessObject;
using BusinessObject.Filter;
using BusinessObject.Query;
using PRN232.Lab1.Repo.Paging;
using PRN232.Lab1.Repo.UnitOfWork;
using PRN232.Lab1.Service.Interfaces;

namespace PRN232.Lab1.Service.Implement;

public class MedicineInfomationService : IMedicineInfomationService
{
    private readonly IUnitOfWork _unitOfWork;

    public MedicineInfomationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MedicineInformation>> GetAll()
    {
        return await _unitOfWork.MedicineInfomationReposiroty.GetAllAsync();
    }

    public async Task<PaginationResult<MedicineInformation>> GetMedicineInfomationFiltered(MedicineFilter filter, Query query)
    {
        return await _unitOfWork.MedicineInfomationReposiroty.GetAllFilteredAsync(filter, query.PageNumber, query.PageSize);
    }
}
