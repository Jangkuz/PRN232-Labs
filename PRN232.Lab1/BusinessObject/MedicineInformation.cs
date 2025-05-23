namespace BusinessObject;

public partial class MedicineInformation : BaseEntity<string>
{
    //public string Id { get; set; } = null!;

    public string MedicineName { get; set; } = null!;

    public string ActiveIngredients { get; set; } = null!;

    public string? ExpirationDate { get; set; }

    public string DosageForm { get; set; } = null!;

    public string WarningsAndPrecautions { get; set; } = null!;

    public string? ManufacturerId { get; set; } = string.Empty;

    public virtual Manufacturer? Manufacturer { get; set; }
}
