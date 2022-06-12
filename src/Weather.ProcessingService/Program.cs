using Serilog;
using Weather.ProcessingService.BL.Services;
using Weather.ProcessingService.BL.Services.Interfaces;
using Weather.ProcessingService.BL.Storages;
using Weather.ProcessingService.BL.Storages.Interfaces;
using Weather.ProcessingService.HostedServices;
using Weather.ProcessingService.Interceptors;
using Weather.ProcessingService.Options;
using Weather.ProcessingService.Services;
using Weather.ProcessingService.Services.Interfaces;
using Weather.SensorService;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.Configure<AggregatorSettings>(builder.Configuration.GetSection("AggregatorSettings"));

builder.Services.AddScoped<RetryInterceptor>();

// Пришлось сделать singlton для сервисов, т.к. требуются в IHostedService
builder.Services.AddSingleton<IEventService, EventService>();
builder.Services.AddSingleton<IAggregatingService, AggregatingService>();

builder.Services.AddSingleton<ILocalRequestQueueService, LocalRequestQueueService>();
builder.Services.AddSingleton<IEventStorage, EventStorage>();
builder.Services.AddSingleton<IAggregatingStorage, AggregatingStorage>();

builder.Services.AddHostedService<AggregatorWorker>();
builder.Services.AddHostedService<StreamReaderHostedService>();

builder.Services.AddControllers();
builder.Services.AddGrpcClient<Generator.GeneratorClient>(
           options =>
           {
               options.Address = new Uri("https://localhost:7200/"); 
           }).AddInterceptor<RetryInterceptor>();;

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
        // routeBuilder.MapGrpcService<GeneratorService>();
    });

app.Run();
