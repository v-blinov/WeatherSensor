using Weather.ProcessingService.BL.Models;

namespace Weather.ProcessingService.BL.Storages.Interfaces;

public interface IAggregatingStorage
{
    void Add(AggregatedData aggregatedData);

    IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatedData();
    IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatedDataForPeriod(Period period);
    
    IEnumerable<AggregatedData> GetAggregatedDataBySensorId(Guid sensorId);
    IEnumerable<AggregatedData> GetAggregatedDataBySensorIdForPeriod(Guid sensorId, Period period);
    
    IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatingData(IEnumerable<Guid> sensorIds);
    IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatingDataForPeriod(IEnumerable<Guid> sensorIds, Period period);
}
