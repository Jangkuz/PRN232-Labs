namespace API.Models;

public class Press
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Category Category { get; set; }
}
