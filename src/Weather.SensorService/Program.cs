using Serilog;
using Weather.SensorService.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddGrpcClient<Weather.SensorService.Generator.GeneratorClient>("IndoorSensor", options =>
    { options.Address = new Uri("https://localhost:7235"); });
builder.Services.AddGrpcClient<Weather.SensorService.Generator.GeneratorClient>("OutdoorSensor", options =>
    { options.Address = new Uri("https://localhost:7235"); });


builder.Services.AddMvcCore();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(
    routeBuilder =>
    {
        // routeBuilder.MapControllers();
        routeBuilder.MapGrpcService<GeneratorService>();
    });

app.Run();