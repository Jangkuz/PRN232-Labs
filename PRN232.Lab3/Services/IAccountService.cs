using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.ResultPattern;
using PRN231_SU25_SE181818.api.DTO.Login;
using Repositories.Entities;

namespace Services;

public interface IAccountService
{
    public Task<Result<LoginResponse>> LoginAsync(string email, string password);
}
