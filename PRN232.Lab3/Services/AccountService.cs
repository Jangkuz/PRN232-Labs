using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Constant;
using BusinessObjects.ResultPattern;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN231_SU25_SE181818.api.DTO.Login;
using Repositories.Entities;
using Repositories.UnitOfWork;

namespace Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponse>> LoginAsync(string email, string password)
    {
        try
        {
            var repo = _unitOfWork.GetRepo<SystemAccount, int>();

            var account = await repo.FindAsync(a =>
                a.Email.ToLower().Equals(email.ToLower())
                && a.Password.Equals(password)
                );

            if (account == null)
            {
                return SystemError.ResourceNotFound("Invalid email or password")
                    .ToGeneric<LoginResponse>();
            }

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, account.Email),
                    new Claim(ClaimName.Role, account.Role?.ToString()),
                    new Claim(ClaimName.AccountId, account.Id.ToString()),
                };

            var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
            var signCredential = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

            var preparedToken = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(16),
                signingCredentials: signCredential);

            var generatedToken = new JwtSecurityTokenHandler().WriteToken(preparedToken);
            var role = account.Role.ToString();
            var accountId = account.Id.ToString();

            return new Result<LoginResponse>
            {
                HtmlStatus = 200,
                IsSuccess = true,
                Data = new LoginResponse
                {
                    Role = role,
                    Token = generatedToken
                }
            };

        }
        catch (Exception ex)
        {
            return SystemError.InternalServerError(ex.Message)
                .ToGeneric<LoginResponse>();
        }
    }
}
