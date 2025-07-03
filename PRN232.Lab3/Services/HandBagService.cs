using BusinessObjects.DTO.HandBag;
using BusinessObjects.Entities;
using BusinessObjects.ResultPattern;
using Microsoft.EntityFrameworkCore;
using Repositories.UnitOfWork;
using Services.Helper;

namespace Services;

public class HandBagService : IHandBagService
{
    private readonly IUnitOfWork _unitOfWork;

    public HandBagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Handbag>> CreateHandBagAsync(CreateUpdateHandBagDTO handBagDTO)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Handbag, int>();

            var brand = _unitOfWork.GetRepo<Brand, int>()
                .GetById(handBagDTO.BrandId);

            if (brand is null)
            {
                return SystemError.ResourceNotFound("Brand not found").ToGeneric<Handbag>();
            }

            var handbag = handBagDTO.ToHandBag();

            var listHb = await repo.GetAllAsync();
            handbag.Id = listHb.Max(hb => hb.Id) + 1;

            handbag = await repo.AddAsync(handbag);
            await _unitOfWork.SaveChangesAsync();

            return new Result<Handbag>
            {
                HtmlStatus = 201,
                IsSuccess = true,
                Data = handbag
            };
        }
        catch (Exception ex)
        {
            return SystemError.InternalServerError(ex.Message)
                .ToGeneric<Handbag>();
        }
    }

    public async Task<Result> DeleteHandBagAsync(int id)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Handbag, int>();
            var handbag = await repo.GetByIdAsync(id);

            if (handbag == null)
            {
                return (Result<Handbag>)SystemError.ResourceNotFound("HandBag not found");
            }

            repo.Delete(handbag);
            await _unitOfWork.SaveChangesAsync();
            return new Result
            {
                HtmlStatus = 200,
                IsSuccess = true,
            };
        }
        catch (Exception ex)
        {
            return SystemError.InternalServerError(ex.Message)
                .ToGeneric<Handbag>();
        }
    }

    public async Task<Result<IEnumerable<Handbag>>> GetAllHandBagAsync()
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Handbag, int>();
            var handbags = await repo.GetAllAsync(q => q.Include(hb => hb.Brand));

            return new Result<IEnumerable<Handbag>>
            {
                HtmlStatus = 200,
                IsSuccess = true,
                Data = handbags.AsEnumerable()
            };
        }
        catch (Exception ex)
        {
            return SystemError.InternalServerError(ex.Message)
                .ToGeneric<IEnumerable<Handbag>>();
        }
    }

    public async Task<Result<Handbag>> GetSingleHandBagAsync(int id)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Handbag, int>();
            var handbag = await repo.GetByIdAsync(id, q => q.Include(hb => hb.Brand));

            if (handbag == null)
            {
                return (Result<Handbag>)SystemError.ResourceNotFound("HandBag not found");
            }

            return new Result<Handbag>
            {
                HtmlStatus = 200,
                IsSuccess = true,
                Data = handbag
            };
        }
        catch (Exception ex)
        {
            return SystemError.InternalServerError(ex.Message)
                .ToGeneric<Handbag>();
        }
    }

    public async Task<Result<Handbag>> UpdateHandBagAsync(int id, CreateUpdateHandBagDTO handBagDTO)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Handbag, int>();
            var handbag = await repo.GetByIdAsync(id);

            if (handbag == null)
            {
                return (Result<Handbag>)SystemError.ResourceNotFound("HandBag not found");
            }

            handbag.ModelName = handBagDTO.ModelName;
            handbag.Price = handBagDTO.Price;
            handbag.Material = handBagDTO.Material;
            handbag.Price = handBagDTO.Price;

            repo.Update(handbag);
            await _unitOfWork.SaveChangesAsync();
            return new Result<Handbag>
            {
                HtmlStatus = 200,
                IsSuccess = true,
                Data = handbag
            };
        }
        catch (Exception ex)
        {
            return SystemError.InternalServerError(ex.Message)
                .ToGeneric<Handbag>();
        }
    }

    public async Task<Result<IEnumerable<Handbag>>> SearchHandBagAsync(string modelName, string material)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<Handbag, int>();

            var handbags = await repo.GetAllAsync(
                include: i => i.Include(hb => hb.Brand),
                filter: f => f.ModelName.ToLower().Contains(modelName)
                && f.Material.ToLower().Contains(material),
                orderBy: null
                );

            var grouping = handbags
                .GroupBy(h => h.BrandId)
                .AsEnumerable();

            var listGroups = new List<Handbag>();
            foreach (var group in grouping)
            {
                //var name = _unitOfWork.GetRepo<Brand, int>()
                //    .GetById((int)group.Key)?.BrandName;
                //var search = new SearchHandBagDTO
                //{
                //    BrandName = name,
                //    Handbags = group.ToList()
                //};
                listGroups.AddRange(group.ToList());
            }

            await _unitOfWork.SaveChangesAsync();
            return new Result<IEnumerable<Handbag>>
            {
                HtmlStatus = 200,
                IsSuccess = true,
                Data = listGroups
            };
        }
        catch (Exception ex)
        {
            return SystemError.InternalServerError(ex.Message)
                .ToGeneric<IEnumerable<Handbag>>();
        }
    }
}
