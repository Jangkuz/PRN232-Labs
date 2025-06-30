using BusinessObjects.Constant;
using BusinessObjects.DTO.HandBag;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repositories.Entities;
using Services;
using System.Threading.Tasks;


namespace PRN231_SU25_SE181818.api.Controllers;

//[Route("api/handbags")]
[ApiController]
public class HandBagsController : ODataController
{
    private readonly IHandBagService _handBagService;

    public HandBagsController(IHandBagService handBagService)
    {
        _handBagService = handBagService;
    }

    [EnableQuery]
    [Authorize(Policy = Policy.AnyWithToken)]
    [HttpGet("/api/handbags")]
    public async Task<ActionResult<IEnumerable<Handbag>>> GetAllHandbags()
    {
        var result = await _handBagService.GetAllHandBagAsync();

        if (!result.IsSuccess)
        {
            return StatusCode(result.HtmlStatus, result.Error);
        }
        return StatusCode(result.HtmlStatus, result.Data);
    }

    [Authorize(Policy = Policy.AnyWithToken)]
    [HttpGet("/api/handbags/{id}")]
    public async Task<ActionResult<Handbag>> GetSingleHandBag(int id)
    {
        var result = await _handBagService.GetSingleHandBagAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.HtmlStatus, result.Error);
        }
        return StatusCode(result.HtmlStatus, result.Data);
    }

    [Authorize(Policy = Policy.AdminOrMod)]
    [HttpPost("/api/handbags")]
    public async Task<ActionResult<Handbag>> CreateHandBag([FromBody] CreateUpdateHandBagDTO createDto)
    {
        var result = await _handBagService.CreateHandBagAsync(createDto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.HtmlStatus, result.Error);
        }
        return StatusCode(result.HtmlStatus, result.Data);
    }

    [Authorize(Policy = Policy.AdminOrMod)]
    [HttpPut("/api/handbags/{id}")]
    public async Task<ActionResult<Handbag>> UpdateHandBag(int id, [FromBody] CreateUpdateHandBagDTO updateDto)
    {

        var result = await _handBagService.UpdateHandBagAsync(id, updateDto);

        if (!result.IsSuccess)
        {
            return StatusCode(result.HtmlStatus, result.Error);
        }
        return StatusCode(result.HtmlStatus, result.Data);
    }

    [Authorize(Policy = Policy.AdminOrMod)]
    [HttpDelete("/api/handbags/{id}")]
    public async Task<ActionResult> DeleteHandbag(int id)
    {
        var result = await _handBagService.DeleteHandBagAsync(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.HtmlStatus, result.Error);
        }
        return StatusCode(result.HtmlStatus);
    }
}
