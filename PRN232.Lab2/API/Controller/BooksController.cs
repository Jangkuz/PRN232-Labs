using API.DAL;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Namespace;

// [Route("api/[controller]")]
// [ApiController]
public class BooksController : ODataController
{
    private readonly BookStoreContext db;
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

    // [HttpGet]
    [EnableQuery(PageSize = 4)]
    public IActionResult Get()
    {
        return Ok(db.Books);
    }

    [HttpPost("api/Books")]
    // [EnableQuery]
    public IActionResult Post([FromBody] Book book)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (book == null)
        {
            return BadRequest("Book cannot be null");
        }

        Book? b = db.Books.FirstOrDefault(b => b.Id == book.Id);
        if (b != null)
        {
            return BadRequest("Book with the same ID already exists");
        }

        //Press? p = db.Presses.Find(book.Press.Id);
        Press? p = db.Presses.AsTracking().FirstOrDefault(p => p.Id == book.Press.Id);
        if (p == null)
        {
            return NotFound("Press not found");
        }

        b = book;
        b.Press = p;

        db.Books.Add(b);
        db.SaveChanges();
        return Created(b);
    }

    [HttpPut]
    public IActionResult Update([FromBody] Book book)
    {
        Book? b = db.Books.FirstOrDefault(c => c.Id == book.Id);
        if (b == null)
        {
            return NotFound();
        }

        Press? p = db.Presses.Find(book.Press.Id);
        if (p == null)
        {
            return NotFound();
        }

        b = book;
        b.Press = p;

        db.Books.Update(b);
        db.SaveChanges();
        return Ok(b);
    }


    [HttpDelete]
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
