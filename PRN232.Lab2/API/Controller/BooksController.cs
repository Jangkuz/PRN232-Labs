using API.DAL;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyApp.Namespace;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ODataController
{
    private BookStoreContext db;
    public BooksController(BookStoreContext context)
    {
        db = context;
        db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        if (context.Books.Count() == 0)
        {
            foreach (var b in DataSource.GetBooks())
            {
                context.Books.Add(b);
                context.Presses.Add(b.Press);
                context.SaveChanges();
            }
        }
    }

    [HttpGet]
    [EnableQuery(PageSize = 3)]
    public IActionResult Get()
    {
        return Ok(db.Books);
    }

    [HttpGet("/Test")]
    [EnableQuery]
    public IActionResult Get(int key, string version)
    {
        return Ok(db.Books.FirstOrDefault(c => c.Id == key));
    }

    [HttpPost]
    [EnableQuery]
    public IActionResult Post([FromBody] Book book)
    {
        db.Books.Add(book);
        db.SaveChanges();
        return Created(book);
    }

    [HttpDelete]
    [EnableQuery]
    public IActionResult Delete([FromBody] int key)
    {
        Book? b = db.Books.FirstOrDefault(c => c.Id == key);
        if (b == null)
        {
            return NotFound();
        }
        db.Books.Remove(b);
        db.SaveChanges();
        return Ok();
    }
}
