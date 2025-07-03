using BusinessObjects.ResultPattern;
using PRN231_SU25_SE181818.api.DTO.Login;

namespace Services;

public interface IAccountService
{
    public Task<Result<LoginResponse>> LoginAsync(string email, string password);
}
