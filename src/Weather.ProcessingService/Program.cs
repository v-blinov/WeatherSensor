using Serilog;
using Weather.ProcessingService.BL.Storages;
using Weather.ProcessingService.BL.Storages.Interfaces;
using Weather.ProcessingService.Options;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.Configure<AggregatorSettings>(builder.Configuration.GetSection("AggregatorSettings"));

builder.Services.AddSingleton<IEventStorage, EventStorage>();
builder.Services.AddSingleton<IAggregatingStorage, AggregatingStorage>();

builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddMvcCore();

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
    });

app.Run();
