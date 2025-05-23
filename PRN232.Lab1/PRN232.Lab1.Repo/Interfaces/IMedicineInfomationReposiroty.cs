using BusinessObject;
using BusinessObject.Filter;
using PRN232.Lab1.Repo.Paging;

namespace PRN232.Lab1.Repo.Interfaces;

public interface IMedicineInfomationReposiroty : IGenericRepository<MedicineInformation, string>
{
    Task<PaginationResult<MedicineInformation>> GetAllFilteredAsync(MedicineFilter filter, int pageNumber, int pageSize);
}
