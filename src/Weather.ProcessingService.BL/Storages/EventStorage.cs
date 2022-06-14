using System.Collections.Concurrent;
using Weather.ProcessingService.BL.Models;
using Weather.ProcessingService.BL.Storages.Interfaces;

namespace Weather.ProcessingService.BL.Storages;

public class EventStorage : IEventStorage
{
    private readonly ConcurrentDictionary<Guid, List<Event>> _storage = new();

    private readonly object _locker = new();
    
    public void Add(Event @event)
    {
        // AddOrUpdate вроде не обеспечивает нужной защищенности, поэтому lock
        lock(_locker)
        {
            if(_storage.ContainsKey(@event.SensorId))
                _storage[@event.SensorId].Add(@event);
            else 
                _storage[@event.SensorId] = new List<Event> { @event };
        }
    }

    public IEnumerable<Event> GetEvents(Guid sensorId)
    {
        return _storage[sensorId];
    }

    public IDictionary<Guid, IEnumerable<Event>> GetEvents(IEnumerable<Guid> sensorIds)
    {
        if(!sensorIds.Any())
            throw new AggregateException($"param {nameof(sensorIds)} couldn't be empty");

        var eventsBySensors = new Dictionary<Guid, IEnumerable<Event>>(sensorIds.Count());
        foreach(var sensorId in sensorIds)
            eventsBySensors[sensorId] = GetEvents(sensorId);

        return eventsBySensors;
    }

    public IDictionary<Guid, IEnumerable<Event>> GetEventsForPeriod(Period period)
    {
        var eventsBySensors = new Dictionary<Guid, IEnumerable<Event>>();
        foreach(var sensorId in _storage.Keys)
        {
            eventsBySensors[sensorId] = _storage[sensorId].Where(e => period.From <= e.CreatedAt && e.CreatedAt <= period.To).ToArray();
        }

        return eventsBySensors;
    }

    public IEnumerable<Event> GetEventsForPeriod(Guid sensorId, Period period)
    {
        var events = _storage[sensorId];
        return events.Where(e => period.From <= e.CreatedAt && e.CreatedAt <= period.To).ToArray();
    }

    public IDictionary<Guid, IEnumerable<Event>> GetEventsForPeriod(IEnumerable<Guid> sensorIds, Period period)
    {
        if(!sensorIds.Any())
            throw new AggregateException($"param {nameof(sensorIds)} couldn't be empty");

        var eventsBySensors = new Dictionary<Guid, IEnumerable<Event>>(sensorIds.Count());
        foreach(var sensorId in sensorIds)
            eventsBySensors[sensorId] = GetEventsForPeriod(sensorId, period);

        return eventsBySensors;
    }
}
