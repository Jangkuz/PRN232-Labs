namespace BusinessObjects.Entities;

public partial class Brand : BaseEntity<int>
{
    //public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? Country { get; set; }

    public int? FoundedYear { get; set; }

    public string? Website { get; set; }

    public virtual ICollection<Handbag> Handbags { get; set; } = new List<Handbag>();
}
