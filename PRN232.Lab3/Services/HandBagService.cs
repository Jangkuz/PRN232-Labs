using BusinessObjects.DTO.HandBag;
using BusinessObjects.ResultPattern;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.UnitOfWork;
using Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var handbag = handBagDTO.ToHandBag();

            var handbags = await repo.AddAsync(handbag);
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
            return (Result<Handbag>)SystemError.InternalServerError(ex.Message);
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
            return (Result<Handbag>)SystemError.InternalServerError(ex.Message);
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
            return (Result<IEnumerable<Handbag>>)SystemError.InternalServerError(ex.Message);
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
            return (Result<Handbag>)SystemError.InternalServerError(ex.Message);
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
            return (Result<Handbag>)SystemError.InternalServerError(ex.Message);
        }
    }
}
