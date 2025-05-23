using BusinessObject;
using BusinessObject.Filter;
using BusinessObject.Query;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Repo.Paging;
using PRN232.Lab1.Service.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN232.Lab1.Api.Controllers
{
    [Route("api/managed-manufacturers")]
    [ApiController]
    public class ManufacturerController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }
        [HttpGet()]
        public async Task<ActionResult<PaginationResult<Manufacturer>>> GetAllFiltered(
            [FromQuery] ManufacturerFilter filter,
            [FromQuery] Query query
            )
        {
            return Ok(await _manufacturerService.GetManufacturersFiltered(filter, query));
        }

        // GET api/<ManufacturerController>/5
        [HttpGet("{id}")]
        public async Task<Manufacturer?> Get(string id)
        {
            return await _manufacturerService.GetManufacturerById(id);
        }

        // POST api/<ManufacturerController>
        [HttpPost]
        public async Task Post([FromBody] Manufacturer manufacturer)
        {
            await _manufacturerService.CreateManufacturer(manufacturer);
        }

        // PUT api/<ManufacturerController>/5
        [HttpPut("{id}")]
        public void Put([FromBody] Manufacturer manufacturer)
        {
            _manufacturerService.UpdateManufacturer(manufacturer);
        }

        // DELETE api/<ManufacturerController>/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _manufacturerService.DeleteManufacturer(id);
        }
    }
}
