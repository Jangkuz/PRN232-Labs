using API.DAL;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MyApp.Namespace;

// [Route("api/[controller]")]
// [ApiController]
public class PressesController : ODataController
{
    private BookStoreContext db;
    public PressesController(BookStoreContext context)
    {
        db = context;
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
    [EnableQuery(PageSize = 3)]
    public IActionResult Get()
    {
        return Ok(db.Presses);
    }

    [HttpPost]
    // [EnableQuery]
    public IActionResult Post([FromBody] Press press)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Press? p = db.Presses.FirstOrDefault(p => p.Id == press.Id);
        if (p != null)
        {
            return BadRequest("Press with the same ID already exists");
        }

        db.Presses.Add(press);
        db.SaveChanges();

        return Created(press);
    }
}
