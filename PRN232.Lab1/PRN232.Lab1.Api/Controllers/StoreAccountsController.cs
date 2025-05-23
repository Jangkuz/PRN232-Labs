using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab1.Service.Interfaces;

namespace PRN232.Lab1.Api.Controllers
{
    [Route("api/managed-accounts")]
    [ApiController]
    public class StoreAccountsController : ControllerBase
    {
        private readonly IStoreAccountService _storeAccountService;

        public StoreAccountsController(IStoreAccountService storeAccountService)
        {
            _storeAccountService = storeAccountService;
        }

        // GET: api/StoreAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoreAccount>> GetStoreAccount(int id)
        {
            return Ok(await _storeAccountService.GetAccountByIdAsync(id));
        }
    }
}
