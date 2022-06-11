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


builder.Services.AddGrpc();
builder.Services.AddControllers();
// builder.Services.AddMvcCore();

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