using ObserverLibrary.Interfaces;
using Weather.SensorService.BL.Enums;
using Weather.SensorService.BL.Models.Interfaces;

namespace Weather.SensorService.BL.Models;

public class Sensor : ISensor, IPublisher
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Event State { get; set; }
    public SensorSettings SensorSettings { get; init; }
    
    private List<ISubscriber> _subscribers = new();
    
    public Sensor(Event state, SensorSettings sensorSettings)
    {
        State = state;                   // TODO : Заполнить из конфига
        SensorSettings = sensorSettings; // TODO : Заполнить из конфига
    }

    #region Generator

    public async Task StreamGeneration(CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(SensorSettings.WorkInterval, cancellationToken);
            var @event = GenerateEvent();

            var observerEvent = new ObserverLibrary.Models.Event
            {
                SensorId = Id,
                CreatedAt = @event.CreatedAt,
                Temperature = @event.EventData.Temperature,
                AirHumidity = @event.EventData.AirHumidity,
                Co2 = @event.EventData.Co2
            };
            
            Notify(observerEvent);
        }
    }
    
    public Event GenerateEvent()
    {
        var random = new Random();
        return new Event
        {
            CreatedAt = DateTime.UtcNow,
            EventData = SensorSettings.Type switch
            {
                SensorType.Indoor => GenerateIndoorEventData(random),
                SensorType.Outdoor => GenerateOutdoorEventData(random),
                _ => State.EventData
            }
        };
    }

    private EventData GenerateIndoorEventData(Random random)
    {
        var state = State.EventData;
        return new EventData
        {
            Temperature = state.Temperature + (random.Next(-2, 2) * 0.1), 
            AirHumidity = state.AirHumidity + random.Next(-2, 2), 
            Co2 = state.Co2 + random.Next(-70, 70)
        };
    }

    private EventData GenerateOutdoorEventData(Random random)
    {
        var state = State.EventData;
        return new EventData
        {
            Temperature = state.Temperature + (random.Next(-5, 5) * 0.1),
            AirHumidity = state.AirHumidity + random.Next(-2, 2),
            Co2 = state.Co2 > 600 ? state.Co2 - random.Next(20)
                : state.Co2 < 300 ? state.Co2 + random.Next(20)
                : state.Co2 + random.Next(-30, 30)
        };
    }

    #endregion
    
    #region Observer

    public void Subscribe(ISubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(ISubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Notify(ObserverLibrary.Models.Event @event)
    {
        foreach(var observer in _subscribers)
            observer.Update(@event);
    }

    #endregion
}
