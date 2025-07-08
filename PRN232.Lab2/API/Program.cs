using System.Text.Json.Serialization;
using API.DAL;
using API.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataBookStore.Models;

var builder = WebApplication.CreateBuilder(args);
var odataBuilder = new ODataConventionModelBuilder();
odataBuilder.EntitySet<Book>("Books");
odataBuilder.EntitySet<Press>("Presses");

// var edmModel = EdmModelBuilder.GetEdmModel();
var edmModel = odataBuilder.GetEdmModel();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
builder.Services.AddDbContext<BookStoreContext>(
    options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

// builder.Services.AddControllers();

builder.Services.AddControllers()
.AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    })
.AddOData(option =>
    option.Select().Filter().Count().OrderBy().Expand().Count().SetMaxTop(100)
    .AddRouteComponents(
        "odata",
        edmModel
        )
    );


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// app.UseHttpsRedirection();

app.UseODataRouteDebug(); // Use to see which OData routes are available

app.UseODataBatching();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Test middleware
app.Use(next => context =>
{
    var endpoint = context.GetEndpoint();
    if (endpoint == null)
    {
        return next(context);
    }

    IEnumerable<string> templates;
    IODataRoutingMetadata? metadata = endpoint.Metadata.GetMetadata<IODataRoutingMetadata>();
    if (metadata != null)
    {
        templates = metadata.Template.GetTemplates();
    }

    return next(context);
});

app.MapControllers();

app.Run();
