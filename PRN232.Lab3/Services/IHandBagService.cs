using BusinessObjects.DTO.HandBag;
using BusinessObjects.Entities;
using BusinessObjects.ResultPattern;

namespace Services;

public interface IHandBagService
{
    Task<Result<IEnumerable<Handbag>>> GetAllHandBagAsync();
    Task<Result<Handbag>> GetSingleHandBagAsync(int id);
    Task<Result<Handbag>> CreateHandBagAsync(CreateUpdateHandBagDTO handBagDTO);
    Task<Result<Handbag>> UpdateHandBagAsync(int id, CreateUpdateHandBagDTO handBagDTO);
    Task<Result> DeleteHandBagAsync(int id);
    Task<Result<IEnumerable<Handbag>>> SearchHandBagAsync(string modelName, string material);
}
