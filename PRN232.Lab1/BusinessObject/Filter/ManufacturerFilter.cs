namespace BusinessObject.Filter;
public class ManufacturerFilter
{
    public string? CountryOfOrigin { get; set; }
    public int? MinYearEstablished { get; set; } = null;
    public int? MaxYearEstablished { get; set; } = null;

    // Sort key and direction
    public string? SortBy { get; set; }       // "Name", "Year"
    public bool SortDesc { get; set; } = false;
}
