using BusinessObjects.Constant;
using BusinessObjects.Entities;
using BusinessObjects.ResultPattern;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories;
using Repositories.Implements;
using Repositories.UnitOfWork;
using Services;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PRN231_SU25_SE181818.api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var odataBuilder = new ODataConventionModelBuilder();
        odataBuilder.EntitySet<Handbag>("Handbags");
        odataBuilder.EntitySet<Brand>("Brands");

        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();

        builder.Services.AddDbContext<Summer2025HandbagDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Add services to the container.
        builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<IHandBagService, HandBagService>();

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            })
            .AddOData(
                options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
                    "odata",
                    odataBuilder.GetEdmModel()
                )
            );


        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.First().ErrorMessage
                    );

                var response = SystemError.InvalidInput("").Error;

                foreach (var key in errors.Keys)
                {
                    response.Message += $"{errors[key]} ";
                }

                response.Message = response.Message.Trim();

                return new BadRequestObjectResult(response);
            };
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
                };


                //Customize JWT response
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        // Skip the default response
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        var response = SystemError.InvalidToken("You are not authorized to access this resource.").Error;

                        var result = JsonSerializer.Serialize(response);

                        return context.Response.WriteAsync(result);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";

                        var response = SystemError.PermissionDeny("You do not have permission to perform this action").Error;

                        var json = JsonSerializer.Serialize(response);

                        return context.Response.WriteAsync(json);
                    }

                };
            });

        // Add Swagger JWT configuration
        builder.Services.AddSwaggerGen(c =>
        {
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "JWT Authentication for API",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT"
            };

            c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                        {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                        },
                    new string[] {}
                }
            };

            c.AddSecurityRequirement(securityRequirement);
        });


        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policy.AdminOrMod, policyBuilder => policyBuilder.RequireAssertion(
                    context => context.User.HasClaim(claim => claim.Type == ClaimName.Role)
                    && (context.User.FindFirst(claim => claim.Type == ClaimName.Role)?.Value == Role.Admin.ToString()
                    || context.User.FindFirst(claim => claim.Type == ClaimName.Role)?.Value == Role.Moderator.ToString())
                    )
            )
            .AddPolicy(Policy.AnyWithToken, policyBuilder => policyBuilder.RequireAssertion(
                    context => context.User.HasClaim(claim => claim.Type == ClaimName.Role)
                    && (context.User.FindFirst(claim => claim.Type == ClaimName.Role)?.Value == Role.Admin.ToString()
                    || context.User.FindFirst(claim => claim.Type == ClaimName.Role)?.Value == Role.Moderator.ToString()
                    || context.User.FindFirst(claim => claim.Type == ClaimName.Role)?.Value == Role.Developer.ToString()
                    || context.User.FindFirst(claim => claim.Type == ClaimName.Role)?.Value == Role.Memeber.ToString())
                    )
            );


        var app = builder.Build();

        app.UseODataRouteDebug();

        app.UseODataBatching();

        app.UseRouting();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
