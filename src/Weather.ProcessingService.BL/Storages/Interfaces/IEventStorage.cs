using Weather.ProcessingService.BL.Models;

namespace Weather.ProcessingService.BL.Storages.Interfaces;

public interface IEventStorage
{
    void Add(Event @event);
    IEnumerable<Event> GetEvents(Guid sensorId);
    IDictionary<Guid, IEnumerable<Event>> GetEvents(IEnumerable<Guid> sensorIds);
    IDictionary<Guid, IEnumerable<Event>> GetEventsForPeriod(Period period);
    IEnumerable<Event> GetEventsForPeriod(Guid sensorId, Period period);
    IDictionary<Guid, IEnumerable<Event>> GetEventsForPeriod(IEnumerable<Guid> sensorIds, Period period);
}
