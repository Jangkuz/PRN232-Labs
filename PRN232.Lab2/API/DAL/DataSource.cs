using API.Models;

namespace API.DAL;

public static class DataSource
{
    private static IList<Book> listBooks { get; set; } = default!;
    public static IList<Book> GetBooks()
    {
        if (listBooks != null)
        {
            return listBooks;
        }
        listBooks = new List<Book>();
        Book book = new Book
        {
            Id = 1,
            ISBN = "978 - 0 - 321 - 87758 - 1",
            Title = "Essential C#5.0",
            Author = "Mark Michaelis",
            Price = 59.99m,
            Location = new Address
            {
                City = "HCM City",
                Street = "D2, Thu Duc District"
            },
            Press = new Press
            {
                Id = 1,
                Name = "Addison-Wesley",
                Category = Category.Book
            }
        };
        listBooks.Add(book);
        book = new Book
        {
            Id = 2,
            ISBN = "978-1-492-03264-9",
            Title = "C# in Depth",
            Author = "Jon Skeet",
            Price = 49.99m,
            Location = new Address
            {
                City = "Boston",
                Street = "5th Avenue, Tech Park"
            },
            Press = new Press
            {
                Id = 2,
                Name = "Manning Publications",
                Category = Category.Book
            }
        };
        listBooks.Add(book);
        book = new Book
        {
            Id = 3,
            ISBN = "978-0-201-63361-0",
            Title = "Design Patterns: Elements of Reusable Object-Oriented Software",
            Author = "Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides",
            Price = 54.99m,
            Location = new Address
            {
                City = "New York",
                Street = "3rd Floor, 123 Design Street"
            },
            Press = new Press
            {
                Id = 3,
                Name = "Addison-Wesley Professional",
                Category = Category.Book
            }
        }; ;
        listBooks.Add(book);
        return listBooks;
    }
}