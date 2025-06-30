using BusinessObjects.DTO.HandBag;
using BusinessObjects.ResultPattern;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services;

public interface IHandBagService
{
    Task<Result<IEnumerable<Handbag>>> GetAllHandBagAsync();
    Task<Result<Handbag>> GetSingleHandBagAsync(int id);
    Task<Result<Handbag>> CreateHandBagAsync(CreateUpdateHandBagDTO handBagDTO);
    Task<Result<Handbag>> UpdateHandBagAsync(int id, CreateUpdateHandBagDTO handBagDTO);
    Task<Result> DeleteHandBagAsync(int id);
}
