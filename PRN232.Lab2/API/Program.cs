using API.DAL;
using API.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);
var modelBuilder = new ODataConventionModelBuilder();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
builder.Services.AddDbContext<BookStoreContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#pragma warning restore CS8602 // Dereference of a possibly null reference.


builder.Services.AddControllers().AddOData(option =>
    option.Select().Filter().Count().OrderBy().Expand().SetMaxTop(100).Count()
    .AddRouteComponents("odata", modelBuilder.GetEdmModel())
    );

//builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseRouting();
app.UseODataBatching();

app.MapControllers();

app.Run();
