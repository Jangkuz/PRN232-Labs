using BusinessObject;
using BusinessObject.Filter;
using BusinessObject.Query;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Repo.Paging;
using PRN232.Lab1.Service.Interfaces;

namespace PRN232.Lab1.Api.Controllers
{
    [Route("api/managed-medicines")]
    [ApiController]
    public class MedicineInformationsController : ControllerBase
    {
        private readonly IMedicineInfomationService _mediicineInfomationService;

        public MedicineInformationsController(IMedicineInfomationService mediicineInfomationService)
        {
            _mediicineInfomationService = mediicineInfomationService;
        }

        // GET: api/MedicineInformations
        [HttpGet()]
        public async Task<ActionResult<PaginationResult<MedicineInformation>>> GetMedicineInformations(
            [FromQuery] MedicineFilter filter,
            [FromQuery] Query query
            )
        {
            return Ok(await _mediicineInfomationService.GetMedicineInfomationFiltered(filter, query));
        }
    }
}
