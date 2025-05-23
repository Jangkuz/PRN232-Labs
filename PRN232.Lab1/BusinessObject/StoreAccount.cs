namespace BusinessObject;

public partial class StoreAccount : BaseEntity<int>
{
    //public int Id { get; set; }

    public string StoreAccountPassword { get; set; } = null!;

    public string? EmailAddress { get; set; }

    public string StoreAccountDescription { get; set; } = null!;

    public int? Role { get; set; }
}
