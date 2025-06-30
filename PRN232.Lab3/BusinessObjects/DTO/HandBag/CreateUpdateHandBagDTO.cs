using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTO.HandBag;

public class CreateUpdateHandBagDTO
{
    public string ModelName { get; set; } = string.Empty;

    public string Material { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

}
