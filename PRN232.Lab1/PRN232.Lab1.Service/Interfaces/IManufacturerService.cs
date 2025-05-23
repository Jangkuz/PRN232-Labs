using BusinessObject;
using BusinessObject.Filter;
using BusinessObject.Query;
using PRN232.Lab1.Repo.Paging;

namespace PRN232.Lab1.Service.Interfaces;

public interface IManufacturerService
{
    Task<IEnumerable<Manufacturer>> GetAllAsync();
    Task<PaginationResult<Manufacturer>> GetManufacturersFiltered(ManufacturerFilter filter, Query query);
    Task<Manufacturer?> GetManufacturerById(string manufacturerId);
    Task<Manufacturer> CreateManufacturer(Manufacturer manufacturer);
    Task<Manufacturer> UpdateManufacturer(Manufacturer manufacturer);
    void DeleteManufacturer(string manufacturerId);
}
