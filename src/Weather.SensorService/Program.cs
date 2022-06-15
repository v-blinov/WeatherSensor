using Serilog;
using Weather.SensorService.BL.Services;
using Weather.SensorService.BL.Services.Interfaces;
using Weather.SensorService.BL.Storages;
using Weather.SensorService.BL.Storages.Interfaces;
using Weather.SensorService.GrpcServices;
using Weather.SensorService.Models;
using Weather.SensorService.Workers;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.Configure<IndoorSettings>(builder.Configuration.GetSection("IndoorSettings"));
builder.Services.Configure<OutdoorSettings>(builder.Configuration.GetSection("OutdoorSettings"));

builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddSingleton<ISensorStorage, SensorStorage>();

builder.Services.AddHostedService<IndoorSensorWorker>();
builder.Services.AddHostedService<OutdoorSensorWorker>();


builder.Services.AddGrpc();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseRouting();
app.UseEndpoints(
    routeBuilder =>
    {
        routeBuilder.MapControllers();
        routeBuilder.MapGrpcService<GeneratorService>();
    });

app.Run();