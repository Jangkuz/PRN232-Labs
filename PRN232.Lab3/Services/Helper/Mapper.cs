using BusinessObjects.DTO.HandBag;
using BusinessObjects.Entities;

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
            Stock = dto.Stock,
            BrandId = dto.BrandId
        };
    }
}
