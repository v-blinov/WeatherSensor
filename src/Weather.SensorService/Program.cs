using Serilog;
using Weather.SensorService.BL.Services;
using Weather.SensorService.BL.Services.Interfaces;
using Weather.SensorService.BL.Storages;
using Weather.SensorService.BL.Storages.Interfaces;
using Weather.SensorService.GrpcServices;
using Weather.SensorService.Models;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.Configure<List<SensorSettings>>(builder.Configuration.GetSection("Sensors"));


builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddSingleton<ISensorStorage, SensorStorage>();


builder.Services.AddGrpcClient<Weather.SensorService.Generator.GeneratorClient>(options =>
{
    options.Address = new Uri("https://localhost:7235");
});

builder.Services.AddControllers();
builder.Services.AddMvcCore();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(
    routeBuilder =>
    {
        routeBuilder.MapControllers();
        routeBuilder.MapGrpcService<GeneratorService>();
    });

app.Run();