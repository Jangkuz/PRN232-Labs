using BusinessObject;
using BusinessObject.Filter;
using PRN232.Lab1.Repo.Paging;

namespace PRN232.Lab1.Repo.Interfaces;

public interface IManufactureRepository : IGenericRepository<Manufacturer, string>
{
    Task<PaginationResult<Manufacturer>> GetAllFilteredAsync(ManufacturerFilter filter, int pageNumber, int pageSize);
}
