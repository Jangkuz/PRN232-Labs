using Microsoft.EntityFrameworkCore;
using PRN232.Lab1.Repo;
using PRN232.Lab1.Repo.Implements;
using PRN232.Lab1.Repo.Interfaces;
using PRN232.Lab1.Repo.UnitOfWork;
using PRN232.Lab1.Service.Implement;
using PRN232.Lab1.Service.Interfaces;
using System.Reflection.PortableExecutable;
using System.Text.Json.Serialization;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PRN232.Lab1.Api;
using System.Diagnostics.Metrics;

namespace ProductStore;

public class Program
{
    public static void Main(string[] args)
    {
        var appBuilder = WebApplication.CreateBuilder(args);
        #region OpenTelemetry

        // Note: Switch between Zipkin/OTLP/Console by setting UseTracingExporter in appsettings.json.
        var tracingExporter = appBuilder.Configuration.GetValue("UseTracingExporter", defaultValue: "CONSOLE").ToUpperInvariant();

        // Note: Switch between Prometheus/OTLP/Console by setting UseMetricsExporter in appsettings.json.
        var metricsExporter = appBuilder.Configuration.GetValue("UseMetricsExporter", defaultValue: "CONSOLE").ToUpperInvariant();

        // Note: Switch between Console/OTLP by setting UseLogExporter in appsettings.json.
        var logExporter = appBuilder.Configuration.GetValue("UseLogExporter", defaultValue: "CONSOLE").ToUpperInvariant();

        // Note: Switch between Explicit/Exponential by setting HistogramAggregation in appsettings.json
        var histogramAggregation = appBuilder.Configuration.GetValue("HistogramAggregation", defaultValue: "EXPLICIT").ToUpperInvariant();

        // Create a service to expose ActivitySource, and Metric Instruments
        // for manual instrumentation
        appBuilder.Services.AddSingleton<InstrumentationSource>();

        // Clear default logging providers used by WebApplication host.
        appBuilder.Logging.ClearProviders();


        appBuilder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: appBuilder.Configuration.GetValue("ServiceName", defaultValue: "otel-test")!,
                    serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString() ?? "unknown",
                    serviceInstanceId: Environment.MachineName))
            .WithTracing(builder =>
            {
                // Tracing

                // Ensure the TracerProvider subscribes to any custom ActivitySources.
                builder
                    .AddSource(InstrumentationSource.ActivitySourceName)
                    .SetSampler(new AlwaysOnSampler())
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                // Use IConfiguration binding for AspNetCore instrumentation options.
                appBuilder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(appBuilder.Configuration.GetSection("AspNetCoreInstrumentation"));

                switch (tracingExporter)
                {
                    //case "ZIPKIN":
                    //    builder.AddZipkinExporter();

                    //    builder.ConfigureServices(services =>
                    //    {
                    //        // Use IConfiguration binding for Zipkin exporter options.
                    //        services.Configure<ZipkinExporterOptions>(builder.Configuration.GetSection("Zipkin"));
                    //    });
                    //    break;

                    case "OTLP":
                        builder.AddOtlpExporter(otlpOptions =>
                        {
                            // Use IConfiguration directly for Otlp exporter endpoint option.
                            otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317"));
                        });
                        break;

                    default:
                        builder.AddConsoleExporter();
                        break;
                }
            })
            .WithMetrics(builder =>
            {
                // Metrics

                // Ensure the MeterProvider subscribes to any custom Meters.
                builder
                    .AddMeter(InstrumentationSource.MeterName)
                    .SetExemplarFilter(ExemplarFilterType.TraceBased)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                switch (histogramAggregation)
                {
                    case "EXPONENTIAL":
                        builder.AddView(instrument =>
                        {
                            return instrument.GetType().GetGenericTypeDefinition() == typeof(Histogram<>)
                                ? new Base2ExponentialBucketHistogramConfiguration()
                                : null;
                        });
                        break;
                    default:
                        // Explicit bounds histogram is the default.
                        // No additional configuration necessary.
                        break;
                }

                switch (metricsExporter)
                {
                    case "PROMETHEUS":
                        builder.AddPrometheusExporter();
                        break;
                    case "OTLP":
                        builder.AddOtlpExporter(otlpOptions =>
                        {
                            // Use IConfiguration directly for Otlp exporter endpoint option.
                            otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                        });
                        break;
                    default:
                        builder.AddConsoleExporter();
                        break;
                }
            })
            .WithLogging(builder =>
            {
                // Note: See appsettings.json Logging:OpenTelemetry section for configuration.

                switch (logExporter)
                {
                    case "OTLP":
                        builder.AddOtlpExporter(otlpOptions =>
                        {
                            // Use IConfiguration directly for Otlp exporter endpoint option.
                            otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317"));
                        });
                        break;
                    default:
                        builder.AddConsoleExporter();
                        break;
                }
            });



        #endregion

        #region DbContext
        string? connectionString = appBuilder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine(connectionString);

        appBuilder.Services.AddDbContext<Lab1PharmaceuticalDbContext>(
            options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(Lab1PharmaceuticalDbContext).Assembly.GetName().Name));
            });
        #endregion

        //====================
        appBuilder.Services.AddControllers();
        appBuilder.Services.AddControllers().AddJsonOptions(option =>
        {
            option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        });
        //=====================
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        appBuilder.Services.AddEndpointsApiExplorer();

        appBuilder.Services.AddScoped<IManufactureRepository, ManufactureRepository>();
        appBuilder.Services.AddScoped<IMedicineInfomationReposiroty, MedicineInfomationRepository>();
        appBuilder.Services.AddScoped<IStoreAccountRepository, StoreAccountRepository>();

        appBuilder.Services.AddScoped<IManufacturerService, ManufacturerService>();
        appBuilder.Services.AddScoped<IMedicineInfomationService, MedicineInfomationService>();
        appBuilder.Services.AddScoped<IStoreAccountService, StoreAccountService>();

        appBuilder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        appBuilder.Services.AddSwaggerGen();

        appBuilder.Services.AddCors(options =>
        {
            options.AddPolicy("DevCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = appBuilder.Build();

        app.UseCors("DevCorsPolicy");

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
