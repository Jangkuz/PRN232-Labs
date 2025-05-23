namespace BusinessObject;

public partial class Manufacturer : BaseEntity<string>
{
    //public string Id { get; set; } = null!;

    public string ManufacturerName { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public int? YearEstablished { get; set; }

    public string ContactInformation { get; set; } = null!;

    public string CountryofOrigin { get; set; } = null!;

    public virtual ICollection<MedicineInformation> MedicineInformations { get; set; } = new List<MedicineInformation>();
}
