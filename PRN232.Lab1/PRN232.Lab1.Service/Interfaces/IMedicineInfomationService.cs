using BusinessObject;
using BusinessObject.Filter;
using BusinessObject.Query;
using PRN232.Lab1.Repo.Paging;

namespace PRN232.Lab1.Service.Interfaces;

public interface IMedicineInfomationService
{
    Task<IEnumerable<MedicineInformation>> GetAll();
    Task<PaginationResult<MedicineInformation>> GetMedicineInfomationFiltered(MedicineFilter filter, Query query);
}
