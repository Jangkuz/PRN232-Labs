using BusinessObjects.DTO.HandBag;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper;

public static class Mapper
{
    public static Handbag ToHandBag(this CreateUpdateHandBagDTO dto)
    {
        return new Handbag
        {
            ModelName = dto.ModelName,
            Material = dto.Material,
            Price = dto.Price,
            Stock = dto.Stock
        };
    }
}
