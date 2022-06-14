using Microsoft.Extensions.Options;
using ObserverLibrary.Interfaces;
using Weather.SensorService.BL.Models;
using Weather.SensorService.BL.Models.Interfaces;
using Weather.SensorService.BL.Storages.Interfaces;
using Weather.SensorService.Models;

namespace Weather.SensorService.Workers;

public class OutdoorSensorWorker : BackgroundService, ISensor
{
    private readonly ILogger<IndoorSensorWorker> _logger;

    public Guid Id { get; init; } = Guid.NewGuid();
    public Event State { get; init; }
    public SensorSettings SensorSettings { get; init; }

    private List<ISubscriber> _subscribers = new();
    
    public OutdoorSensorWorker(
        IOptions<OutdoorSettings> initializeSensorSettings, 
        ILogger<IndoorSensorWorker> logger,
        ISensorStorage sensorStorage)
    {
        _logger = logger;

        var settings = initializeSensorSettings.Value.InitializeSettings;
        SensorSettings = new SensorSettings
        {
            Type = settings.SensorType,
            WorkInterval = settings.WorkInterval
        };
        State = new Event
        {
            CreatedAt = DateTime.UtcNow,
            EventData = new EventData
            {
                Temperature = settings.InitializeEventValues.Temperature,
                AirHumidity = settings.InitializeEventValues.AirHumidity,
                Co2 = settings.InitializeEventValues.Co2
            }
        };
        
        sensorStorage.Add(this);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("{Worker} start executing", nameof(OutdoorSensorWorker));
        
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(SensorSettings.WorkInterval), stoppingToken);
            var @event = GenerateEvent();

            var observerEvent = new ObserverLibrary.Models.EventItem
            {
                SensorId = Id,
                CreatedAt = @event.CreatedAt,
                Temperature = @event.EventData.Temperature,
                AirHumidity = @event.EventData.AirHumidity,
                Co2 = @event.EventData.Co2
            };
            
            Notify(observerEvent);
        }
        
        _logger.LogInformation("{Worker} stop executing", nameof(OutdoorSensorWorker));
    }
    
    public Event GenerateEvent()
    {
        var state = State.EventData;
        var random = new Random();
        
        return new Event
        {
            CreatedAt = DateTime.UtcNow,
            EventData = new EventData
            {
                Temperature = state.Temperature + (random.Next(-5, 5) * 0.1),
                AirHumidity = state.AirHumidity + random.Next(-2, 2),
                Co2 = state.Co2 > 600 ? state.Co2 - random.Next(20)
                    : state.Co2 < 300 ? state.Co2 + random.Next(20)
                    : state.Co2 + random.Next(-30, 30)
            }
        };
    }

    public void Subscribe(ISubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    public void Notify(ObserverLibrary.Models.EventItem eventItem)
    {
        foreach(var subscriber in _subscribers)
            subscriber.UpdateSensorEventsQueue(eventItem);
    }
}