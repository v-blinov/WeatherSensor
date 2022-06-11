using System.Collections.Concurrent;
using Weather.ProcessingService.BL.Models;
using Weather.ProcessingService.BL.Storages.Interfaces;

namespace Weather.ProcessingService.BL.Storages;

public class EventStorage : IEventStorage
{
    private readonly ConcurrentDictionary<Guid, List<Event>> _storage = new();

    public void Add(Event @event)
    {
        if(!_storage.ContainsKey(@event.SensorId))
            _storage[@event.SensorId] = new List<Event>();

        _storage[@event.SensorId].Add(@event);
    }

    public IEnumerable<Event> GetEvents(Guid sensorId)
    {
        if(!_storage.TryGetValue(sensorId, out var events))
            throw new KeyNotFoundException();

        return events.ToArray();
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
        if(!_storage.TryGetValue(sensorId, out var events))
            throw new KeyNotFoundException();

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
