using ObserverLibrary.Interfaces;
using Weather.SensorService.BL.Models.Dtos;
using Weather.SensorService.BL.Models.Interfaces;
using Weather.SensorService.BL.Services.Interfaces;
using Weather.SensorService.BL.Storages.Interfaces;

namespace Weather.SensorService.BL.Services;

public class SensorService : ISensorService
{
    private readonly ISensorStorage _sensorStorage;

    public SensorService(ISensorStorage sensorStorage)
    {
        _sensorStorage = sensorStorage;
    }
    
    public void Add(ISensor sensor)
    {
        _sensorStorage.Add(sensor);
    }

    public void AddRange(IEnumerable<ISensor> sensors)
    {
        _sensorStorage.AddRange(sensors);
    }

    public SensorDto GetSensor(Guid id)
    {
        var sensor = _sensorStorage.GetSensor(id);
        var sensorDto = new SensorDto
        {
            Id = sensor.Id,
            SensorSettings = new SensorSettingDto
            {
                Type = sensor.SensorSettings.Type,
                WorkInterval = sensor.SensorSettings.WorkInterval
            },
            Event = new EventDto
            {
                CreatedAt = sensor.State.CreatedAt,
                EventData = new EventDataDto
                {
                    Temperature = sensor.State.EventData.Temperature,
                    AirHumidity = sensor.State.EventData.AirHumidity,
                    Co2 = sensor.State.EventData.Co2,
                }
            }
        };

        return sensorDto;
    }

    public IEnumerable<SensorDto> GetSensors()
    {
        var sensors = _sensorStorage.GetSensors();
        var sensorsDtos = sensors.Select(sensor => new SensorDto
        {
            Id = sensor.Id,
            SensorSettings = new SensorSettingDto
            {
                Type = sensor.SensorSettings.Type,
                WorkInterval = sensor.SensorSettings.WorkInterval
            },
            Event = new EventDto
            {
                CreatedAt = sensor.State.CreatedAt,
                EventData = new EventDataDto
                {
                    Temperature = sensor.State.EventData.Temperature,
                    AirHumidity = sensor.State.EventData.AirHumidity,
                    Co2 = sensor.State.EventData.Co2,
                }
            }
        }).ToArray();

        return sensorsDtos;
    }

    public void TrySubscribe(ISubscriber subscriber, Guid sensorId)
    {
        var sensor = _sensorStorage.GetSensor(sensorId);
        sensor.Subscribe(subscriber);
    }

    public void TryUnsubscribe(ISubscriber subscriber, Guid sensorId)
    {
        var sensor = _sensorStorage.GetSensor(sensorId);
        sensor.Unsubscribe(subscriber);
    }
}