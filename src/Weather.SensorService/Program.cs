using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
             .ReadFrom.Configuration(builder.Configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
