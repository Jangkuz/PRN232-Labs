using BusinessObject;
using BusinessObject.Filter;
using BusinessObject.Query;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Repo.Paging;
using PRN232.Lab1.Service.Interfaces;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace PRN232.Lab1.Api.Controllers;

[Route("api/managed-medicines")]
[ApiController]
public class MedicineInformationsController : ControllerBase
{
    private readonly IMedicineInfomationService _mediicineInfomationService;

    private readonly Counter<long> filterRequestCounter;
    private readonly Histogram<double> filterProcessingTime;

    public MedicineInformationsController(IMedicineInfomationService mediicineInfomationService,
        InstrumentationSource instrumentationSource)
    {
        _mediicineInfomationService = mediicineInfomationService;
        filterRequestCounter = instrumentationSource.FilterRequesCounter;
        filterProcessingTime = instrumentationSource.FilterProcessingTime;
    }

    // GET: api/MedicineInformations
    [HttpGet()]
    public async Task<ActionResult<PaginationResult<MedicineInformation>>> GetMedicineInformations(
        [FromQuery] MedicineFilter filter,
        [FromQuery] Query query
        )
    {
        filterRequestCounter.Add(1);
        return Ok(await _mediicineInfomationService.GetMedicineInfomationFiltered(filter, query));
    }

    [HttpGet("delayed")]
    public async Task<ActionResult<PaginationResult<MedicineInformation>>> GetMedicineInformationsDelayed(
        [FromQuery] MedicineFilter filter,
        [FromQuery] Query query
        )
    {
        filterRequestCounter.Add(1);

        var stopWatch = Stopwatch.StartNew();
        Thread.Sleep(3000);
        var result = await _mediicineInfomationService.GetMedicineInfomationFiltered(filter, query);
        stopWatch.Stop();
        filterProcessingTime.Record(stopWatch.ElapsedMilliseconds);
        return Ok(result);
    }
}
