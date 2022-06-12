using Weather.ProcessingService.BL.Models;
using Weather.ProcessingService.BL.Models.Dtos;

namespace Weather.ProcessingService.BL.Services.Interfaces;

public interface IAggregatingService
{
    void Add(Guid sensorId, IEnumerable<EventDto> eventDtos, PeriodDto periodDto);
    Dictionary<Guid, IEnumerable<AggregatedData>> GetAllAggregatedItems();
    
    IEnumerable<AggregatedDataDto> GetAggregatedDataForInterval(DateTime from, DateTime to, Guid sensorId);
    IEnumerable<AggregatedDataDto> GetAggregatedDataForInterval(DateTime from, int durationInMinutes, Guid sensorId);
    
    Dictionary<Guid, IEnumerable<AggregatedDataDto>> GetAggregatedDataForInterval(DateTime from, DateTime to);
    Dictionary<Guid, IEnumerable<AggregatedDataDto>> GetAggregatedDataForInterval(DateTime from, int durationInMinutes);
}
