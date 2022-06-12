using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Weather.ProcessingService.BL.Models.Dtos;
using Weather.ProcessingService.BL.Services.Interfaces;
using Weather.ProcessingService.Options;
using Weather.SensorService;

namespace Weather.ProcessingService.HostedServices;

public class AggregatorWorker : BackgroundService
{
    private readonly AggregatorSettings _settings;
    private readonly ILogger<AggregatorWorker> _logger;
    private readonly IEventService _eventService;
    private readonly IAggregatingService _aggregatingService;

    private DateTime lastTimeWork;

    public AggregatorWorker(IOptions<AggregatorSettings> settings, 
        ILogger<AggregatorWorker> logger,
        IEventService eventService, 
        IAggregatingService aggregatingService)
    {
        _settings = settings.Value;
        _logger = logger;
        _eventService = eventService;
        _aggregatingService = aggregatingService;

        var dtNow = DateTime.UtcNow;
        lastTimeWork = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Worker} start executing", nameof(AggregatorWorker));
        
        while(!stoppingToken.IsCancellationRequested)
        {
            var dtNow = DateTime.UtcNow;
            var dtCheck = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0);
            
            var difference = dtCheck - lastTimeWork;
            if(difference.Minutes * 60 <= _settings.AggregationPeriod)
            {
                await Task.Delay(TimeSpan.FromSeconds(_settings.WaitingTime), stoppingToken);
                continue;
            }
            
            // aggregating
            var period = new PeriodDto { From = lastTimeWork, To = dtCheck };
            
            var eventsBySensors = _eventService.GetEventsForPeriod(period);
            foreach(var item in eventsBySensors)
                _aggregatingService.Add(item.Key, item.Value, period);

            lastTimeWork = dtCheck;
        }
        
        _logger.LogInformation("{Worker} stop executing", nameof(AggregatorWorker));
    }
}
