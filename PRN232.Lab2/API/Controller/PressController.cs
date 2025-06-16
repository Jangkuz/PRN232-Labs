using API.DAL;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace MyApp.Namespace;

[Route("odata/[controller]")]
[ApiController]
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
    [HttpGet]
    [EnableQuery]
    public IActionResult Get()
    {
        return Ok(db.Presses);
    }
}
