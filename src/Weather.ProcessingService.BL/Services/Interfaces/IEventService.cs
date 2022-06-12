using Weather.ProcessingService.BL.Models.Dtos;

namespace Weather.ProcessingService.BL.Services.Interfaces;

public interface IEventService
{
    void Add(Guid sensorId, EventDto @event);
    IEnumerable<EventDto> GetEvents(Guid sensorId);
    IDictionary<Guid, IEnumerable<EventDto>> GetEvents(IEnumerable<Guid> sensorIds);
    IDictionary<Guid, IEnumerable<EventDto>> GetEventsForPeriod(PeriodDto periodDto);
    IEnumerable<EventDto> GetEventsForPeriod(Guid sensorId, PeriodDto period);
    IDictionary<Guid, IEnumerable<EventDto>> GetEventsForPeriod(IEnumerable<Guid> sensorIds, PeriodDto period);
}
