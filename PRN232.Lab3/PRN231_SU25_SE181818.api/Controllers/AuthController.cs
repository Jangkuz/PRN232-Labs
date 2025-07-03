using Microsoft.AspNetCore.Mvc;
using PRN231_SU25_SE181818.api.DTO.Login;
using Services;

namespace PRN231_SU25_SE181818.api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AuthController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await _accountService.LoginAsync(loginRequest.Email, loginRequest.Password);

        if (!result.IsSuccess)
        {
            return StatusCode(result.HtmlStatus, result.Error);
        }
        return StatusCode(result.HtmlStatus, result.Data);
    }
}
