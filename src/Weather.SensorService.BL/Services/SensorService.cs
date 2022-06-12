﻿using ObserverLibrary.Interfaces;
using Weather.SensorService.BL.Models;
using Weather.SensorService.BL.Models.Dtos;
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
    
    
    public void Add(SensorDto sensorDto)
    {
        var sensor = new Sensor(new Event 
            {
                CreatedAt = sensorDto.Event.CreatedAt, 
                EventData = new EventData
                {
                    Temperature = sensorDto.Event.EventData.Temperature, 
                    AirHumidity = sensorDto.Event.EventData.AirHumidity, 
                    Co2 = sensorDto.Event.EventData.Co2
                }
            },
            new SensorSettings
            {
                WorkInterval = sensorDto.SensorSettings.WorkInterval, 
                Type = sensorDto.SensorSettings.Type
            });

        _sensorStorage.Add(sensor);
    }

    public void AddRange(IEnumerable<SensorDto> sensorDtos)
    {
        var sensors = sensorDtos.Select(p => new Sensor(new Event 
            {
                CreatedAt = p.Event.CreatedAt, 
                EventData = new EventData
                {
                    Temperature = p.Event.EventData.Temperature, 
                    AirHumidity = p.Event.EventData.AirHumidity, 
                    Co2 = p.Event.EventData.Co2
                }
            },
            new SensorSettings
            {
                WorkInterval = p.SensorSettings.WorkInterval, 
                Type = p.SensorSettings.Type
            })).ToArray();
        
        _sensorStorage.AddRange(sensors);
    }

    public SensorDto GetSensor(Guid id)
    {
        var sensor = _sensorStorage.GetSensor(id);
        if(sensor is null)
            throw new KeyNotFoundException(id.ToString());
        
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

    public IEnumerable<SensorDto> GetSensors(IEnumerable<Guid> ids)
    {
        var sensors = _sensorStorage.GetSensors(ids);
        
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

    public IEnumerable<Guid> GetSensorIds()
    {
        var sensors = _sensorStorage.GetSensors();
        return sensors.Select(sensor => sensor.Id).ToArray();
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