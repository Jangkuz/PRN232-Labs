namespace BusinessObject.Filter;

public class MedicineFilter
{
    public string? MedicineName { get; set; }
    public string? ActiveIngredient { get; set; }
    public string? ManufacturerId { get; set; }

    public string? SortBy { get; set; } //Name, ActiveIngredient, ManufacturerName
    public bool SortDesc { get; set; } = false;
}
