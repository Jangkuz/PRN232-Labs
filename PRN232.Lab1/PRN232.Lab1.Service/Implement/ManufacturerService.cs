using BusinessObject;
using BusinessObject.Filter;
using BusinessObject.Query;
using PRN232.Lab1.Repo.Paging;
using PRN232.Lab1.Repo.UnitOfWork;
using PRN232.Lab1.Service.Interfaces;

namespace PRN232.Lab1.Service.Implement
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManufacturerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<PaginationResult<Manufacturer>> GetManufacturersFiltered(ManufacturerFilter filter, Query query)
        {
            return _unitOfWork.ManufactureRepository.GetAllFilteredAsync(filter, query.PageNumber, query.PageSize);
        }
        public async Task<IEnumerable<Manufacturer>> GetAllAsync()
        {
            return await _unitOfWork.ManufactureRepository.GetAllAsync();
        }

        public async Task<Manufacturer?> GetManufacturerById(string manufacturerId)
        {
            return await _unitOfWork.ManufactureRepository.GetByIdAsync(manufacturerId);
        }

        public async Task<Manufacturer> CreateManufacturer(Manufacturer manufacturer)
        {
            //await _unitOfWork.BeginTransactionAsync();
            var result = await _unitOfWork.ManufactureRepository.AddAsync(manufacturer);
            await _unitOfWork.SaveChangesAsync();

            return result;
        }

        public async void DeleteManufacturer(string manufacturerId)
        {
            //await _unitOfWork.BeginTransactionAsync();
            _unitOfWork.ManufactureRepository.Delete(manufacturerId);
            await _unitOfWork.SaveChangesAsync();
        }



        public async Task<Manufacturer> UpdateManufacturer(Manufacturer manufacturer)
        {
            //await _unitOfWork.BeginTransactionAsync();
            var result = _unitOfWork.ManufactureRepository.Update(manufacturer);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
    }
}
