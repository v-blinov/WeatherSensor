using Weather.ProcessingService.BL.Models;
using Weather.ProcessingService.BL.Models.Dtos;
using Weather.ProcessingService.BL.Services.Interfaces;
using Weather.ProcessingService.BL.Storages.Interfaces;

namespace Weather.ProcessingService.BL.Services;

public class EventService : IEventService
{
    private readonly IEventStorage _eventStorage;

    public EventService(IEventStorage eventStorage)
    {
        _eventStorage = eventStorage;
    }
    
    public void Add(Guid sensorId, EventDto eventDto)
    {
        var @event = new Event
        {
            SensorId = sensorId,
            CreatedAt = eventDto.CreatedAt.ToUniversalTime(),
            Temperature = eventDto.Temperature,
            AirHumidity = eventDto.AirHumidity,
            Co2 = eventDto.Co2
        };
        
        _eventStorage.Add(@event);
    }

    public IEnumerable<EventDto> GetEvents(Guid sensorId)
    {
        var events = _eventStorage.GetEvents(sensorId);
        var eventDtos = events.Select(@event => new EventDto()
        {
            CreatedAt = @event.CreatedAt,
            Temperature = @event.Temperature,
            AirHumidity = @event.AirHumidity,
            Co2 = @event.Co2
        }).ToArray();
        return eventDtos;
    }

    public IDictionary<Guid, IEnumerable<EventDto>> GetEvents(IEnumerable<Guid> sensorIds)
    {
        var eventsBySensors = _eventStorage.GetEvents(sensorIds);
        
        var eventDtosBySensors = new Dictionary<Guid, IEnumerable<EventDto>>(eventsBySensors.Count);
        foreach(var sensor in eventsBySensors)
        {
            eventDtosBySensors[sensor.Key] = sensor.Value.Select(@event => new EventDto()
            {
                CreatedAt = @event.CreatedAt,
                Temperature = @event.Temperature,
                AirHumidity = @event.AirHumidity,
                Co2 = @event.Co2
            }).ToArray();
        }

        return eventDtosBySensors;
    }
    
    public IDictionary<Guid, IEnumerable<EventDto>> GetEventsForPeriod(PeriodDto periodDto)
    {
        // TODO: Add fluent validator in presentation layer
        if (periodDto.From >= periodDto.To)
            throw new ArgumentException($"Datetime From ({periodDto.From}) couldn't bu more then datetime To ({periodDto.To})");
        
        var period = new Period
        {
            From = periodDto.From.ToUniversalTime(), 
            To = periodDto.To.ToUniversalTime()
        };
        
        var eventsBySensors = _eventStorage.GetEventsForPeriod(period);
        
        var eventDtosBySensors = new Dictionary<Guid, IEnumerable<EventDto>>(eventsBySensors.Count);
        foreach(var sensor in eventsBySensors)
        {
            eventDtosBySensors[sensor.Key] = sensor.Value.Select(@event => new EventDto()
            {
                CreatedAt = @event.CreatedAt,
                Temperature = @event.Temperature,
                AirHumidity = @event.AirHumidity,
                Co2 = @event.Co2
            }).ToArray();
        }

        return eventDtosBySensors;
    }
    
    public IEnumerable<EventDto> GetEventsForPeriod(Guid sensorId, PeriodDto periodDto)
    {
        // TODO: Add fluent validator in presentation layer
        if (periodDto.From >= periodDto.To)
            throw new ArgumentException($"Datetime From ({periodDto.From}) couldn't bu more then datetime To ({periodDto.To})");
        
        var period = new Period
        {
            From = periodDto.From.ToUniversalTime(), 
            To = periodDto.To.ToUniversalTime()
        };
        
        var events = _eventStorage.GetEventsForPeriod(sensorId, period);
        var eventDtos = events.Select(@event => new EventDto()
        {
            CreatedAt = @event.CreatedAt,
            Temperature = @event.Temperature,
            AirHumidity = @event.AirHumidity,
            Co2 = @event.Co2
        }).ToArray();
        return eventDtos;
    }

    public IDictionary<Guid, IEnumerable<EventDto>> GetEventsForPeriod(IEnumerable<Guid> sensorIds, PeriodDto periodDto)
    {
        // TODO: Add fluent validator in presentation layer
        if (periodDto.From >= periodDto.To)
            throw new ArgumentException($"Datetime From ({periodDto.From}) couldn't bu more then datetime To ({periodDto.To})");
        
        var period = new Period
        {
            From = periodDto.From.ToUniversalTime(), 
            To = periodDto.To.ToUniversalTime()
        };
        
        var eventsBySensors = _eventStorage.GetEventsForPeriod(sensorIds, period);
        
        var eventDtosBySensors = new Dictionary<Guid, IEnumerable<EventDto>>(eventsBySensors.Count);
        foreach(var sensor in eventsBySensors)
        {
            eventDtosBySensors[sensor.Key] = sensor.Value.Select(@event => new EventDto()
            {
                CreatedAt = @event.CreatedAt,
                Temperature = @event.Temperature,
                AirHumidity = @event.AirHumidity,
                Co2 = @event.Co2
            }).ToArray();
        }

        return eventDtosBySensors;
    }
}
