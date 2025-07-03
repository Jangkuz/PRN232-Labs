using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.DTO.HandBag;

public class CreateUpdateHandBagDTO
{
    [Required]
    [RegularExpression("^([A-Z0-9][a-zA-Z0-9#]*\\s)*([A-Z0-9][a-zA-Z0-9#]*)$", ErrorMessage = "Invalid model name")]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    public string Material { get; set; } = string.Empty;

    [Required]
    [Range((double)decimal.Zero, (double)decimal.MaxValue, ErrorMessage = "Invalid price")]
    public decimal Price { get; set; }

    [Required]
    [Range((double)decimal.Zero, (double)decimal.MaxValue, ErrorMessage = "Invalid stock")]
    public int Stock { get; set; }

    [Required]
    public int BrandId { get; set; }

}
