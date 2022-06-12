using Grpc.Core;
using Weather.ProcessingService.BL.Models.Dtos;
using Weather.ProcessingService.BL.Services.Interfaces;
using Weather.SensorService;

namespace Weather.ProcessingService.HostedServices;

public class StreamReaderHostedService : BackgroundService
{
    private readonly ILogger<StreamReaderHostedService> _logger;
    private readonly IEventService _eventService;
    private readonly IServiceProvider _provider;

    public StreamReaderHostedService(
        ILogger<StreamReaderHostedService> logger,
        IEventService eventService,
        IServiceProvider provider)
    {
        _logger = logger;
        _eventService = eventService;
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<Generator.GeneratorClient>();
        using var eventResponseStream = client.SendEvents(new Metadata(), cancellationToken: stoppingToken);

        
        // не работает..
        while(await eventResponseStream.ResponseStream.MoveNext(stoppingToken))
        {
            var e = eventResponseStream.ResponseStream.Current;

            if(e is null)
                continue;

            var @event = new EventDto
            {
                CreatedAt = e.CreatedAt.ToDateTime(), 
                Temperature = e.Temperature, 
                AirHumidity = e.AirHumidity, 
                Co2 = e.Co2
            };

            var sensorId = new Guid(e.SensorId);
            _eventService.Add(sensorId, @event);
        }

        _logger.LogInformation("{Worker} stop executing", nameof(StreamReaderHostedService));
    }
}
