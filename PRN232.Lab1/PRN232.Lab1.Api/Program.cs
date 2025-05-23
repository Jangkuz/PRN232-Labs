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

namespace ProductStore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        #region OpenTelemetry

        // Note: Switch between Zipkin/OTLP/Console by setting UseTracingExporter in appsettings.json.
        var tracingExporter = builder.Configuration.GetValue("UseTracingExporter", defaultValue: "CONSOLE").ToUpperInvariant();

        // Note: Switch between Prometheus/OTLP/Console by setting UseMetricsExporter in appsettings.json.
        var metricsExporter = builder.Configuration.GetValue("UseMetricsExporter", defaultValue: "CONSOLE").ToUpperInvariant();

        // Note: Switch between Console/OTLP by setting UseLogExporter in appsettings.json.
        var logExporter = builder.Configuration.GetValue("UseLogExporter", defaultValue: "CONSOLE").ToUpperInvariant();

        // Note: Switch between Explicit/Exponential by setting HistogramAggregation in appsettings.json
        var histogramAggregation = builder.Configuration.GetValue("HistogramAggregation", defaultValue: "EXPLICIT").ToUpperInvariant();

        // Create a service to expose ActivitySource, and Metric Instruments
        // for manual instrumentation
        builder.Services.AddSingleton<InstrumentationSource>();

        // Clear default logging providers used by WebApplication host.
        builder.Logging.ClearProviders();


        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: builder.Configuration.GetValue("ServiceName", defaultValue: "otel-test")!,
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
                builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(builder.Configuration.GetSection("AspNetCoreInstrumentation"));

                switch (tracingExporter)
                {
                    case "ZIPKIN":
                        builder.AddZipkinExporter();

                        builder.ConfigureServices(services =>
                        {
                            // Use IConfiguration binding for Zipkin exporter options.
                            services.Configure<ZipkinExporterOptions>(builder.Configuration.GetSection("Zipkin"));
                        });
                        break;

                    case "OTLP":
                        builder.AddOtlpExporter(otlpOptions =>
                        {
                            // Use IConfiguration directly for Otlp exporter endpoint option.
                            otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317"));
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
                            otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
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
                            otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317"));
                        });
                        break;
                    default:
                        builder.AddConsoleExporter();
                        break;
                }
            });



        #endregion

        #region DbContext
        string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine(connectionString);

        builder.Services.AddDbContext<Lab1PharmaceuticalDbContext>(
            options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(Lab1PharmaceuticalDbContext).Assembly.GetName().Name));
            });
        #endregion

        //====================
        builder.Services.AddControllers();
        builder.Services.AddControllers().AddJsonOptions(option =>
        {
            option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        });
        //=====================
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddScoped<IManufactureRepository, ManufactureRepository>();
        builder.Services.AddScoped<IMedicineInfomationReposiroty, MedicineInfomationRepository>();
        builder.Services.AddScoped<IStoreAccountRepository, StoreAccountRepository>();

        builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
        builder.Services.AddScoped<IMedicineInfomationService, MedicineInfomationService>();
        builder.Services.AddScoped<IStoreAccountService, StoreAccountService>();

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevCorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

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
