using Weather.SensorService.BL.Enums;
using Weather.SensorService.BL.Models.Interfaces;

namespace Weather.SensorService.BL.Models;

public class Sensor : ISensor
{
    public Guid Id { get; init; }

    private Event _state;
    private readonly SensorSettings _sensorSettings;
    private List<string> _subscribers = new();
    public Sensor(Event state, SensorSettings sensorSettings)
    {
        _state = state;
        _sensorSettings = sensorSettings;
    }

    #region Generator

    public Event GenerateEvent()
    {
        var random = new Random();
        return new Event
        {
            CreatedAt = DateTime.UtcNow,
            EventData = _sensorSettings.Type switch
            {
                SensorType.Indoor => GenerateIndoorEventData(random),
                SensorType.Outdoor => GenerateOutdoorEventData(random),
                _ => _state.EventData
            }
        };
    }

    private EventData GenerateIndoorEventData(Random random)
    {
        var state = _state.EventData;
        return new EventData
        {
            Temperature = state.Temperature + (random.Next(-2, 2) * 0.1), 
            AirHumidity = state.AirHumidity + random.Next(-2, 2), 
            Co2 = state.Co2 + random.Next(-70, 70)
        };
    }

    private EventData GenerateOutdoorEventData(Random random)
    {
        var state = _state.EventData;
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
}
