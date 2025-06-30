namespace API.Models;

public class Book
{
    public int Id { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Address Location { get; set; } = default!;
    public Press Press { get; set; } = default!;
}