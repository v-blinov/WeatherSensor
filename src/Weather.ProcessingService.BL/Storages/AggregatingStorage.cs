using System.Collections.Concurrent;
using Weather.ProcessingService.BL.Models;
using Weather.ProcessingService.BL.Storages.Interfaces;

namespace Weather.ProcessingService.BL.Storages;

public class AggregatingStorage : IAggregatingStorage
{
    private readonly ConcurrentDictionary<Guid, List<AggregatedData>> _storage = new();

    public void Add(AggregatedData aggregatedData)
    {
        if(!_storage.ContainsKey(aggregatedData.SensorId))
            _storage[aggregatedData.SensorId] = new List<AggregatedData>();

        _storage[aggregatedData.SensorId].Add(aggregatedData);
    }

    public IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatedData()
    {
        var dataBySensors = new Dictionary<Guid, IEnumerable<AggregatedData>>();
        foreach(var sensorsId in _storage.Keys)
            dataBySensors[sensorsId] = _storage[sensorsId].ToArray();
        
        return dataBySensors;
    }
    
    public IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatedDataForPeriod(Period period)
    {
        var dataBySensors = new Dictionary<Guid, IEnumerable<AggregatedData>>();
        foreach(var sensorsId in _storage.Keys)
            dataBySensors[sensorsId] = GetAggregatingDataBySensorInternal(sensorsId, period).ToArray();
        
        return dataBySensors;
    }
    
    public IEnumerable<AggregatedData> GetAggregatedDataBySensorId(Guid sensorId)
    {
        if (!_storage.TryGetValue(sensorId, out var data))
            throw new KeyNotFoundException();

        return data.ToArray();
    }
    
    public IEnumerable<AggregatedData> GetAggregatedDataBySensorIdForPeriod(Guid sensorId, Period period)
    {
        return GetAggregatingDataBySensorInternal(sensorId, period);
    }

    public IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatingData(IEnumerable<Guid> sensorIds)
    {
        var dataBySensors = new Dictionary<Guid, IEnumerable<AggregatedData>>();
        foreach(var sensorId in sensorIds)
        {
            if (!_storage.TryGetValue(sensorId, out var data))
                throw new KeyNotFoundException();

            dataBySensors[sensorId] = data.ToArray();
        }
        
        return dataBySensors;
    }

    public IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatingDataForPeriod(IEnumerable<Guid> sensorIds, Period period)
    {
        var dataBySensors = new Dictionary<Guid, IEnumerable<AggregatedData>>();
        foreach(var sensorId in sensorIds)
        {
            if (!_storage.TryGetValue(sensorId, out var data))
                throw new KeyNotFoundException();

            dataBySensors[sensorId] = GetAggregatingDataBySensorInternal(sensorId, period);
        }
        
        return dataBySensors;
    }
    
    private IEnumerable<AggregatedData> GetAggregatingDataBySensorInternal(Guid sensorId, Period period)
    {
        if(period.From.CompareTo(period.To) > 0)
            throw new ArgumentException($"Datetime From ({period.From}) couldn't bu more then datetime To ({period.To})");

        if(!_storage.TryGetValue(sensorId, out var aggregatingData))
            throw new KeyNotFoundException();

        var from = new DateTime(period.From.Year, period.From.Month, period.From.Day, period.From.Hour, period.From.Minute, 0);
        var to = new DateTime(period.To.Year, period.To.Month, period.To.Day, period.To.Hour, period.To.Minute, 0);

        return aggregatingData.Where(p => from <= p.Period!.From && p.Period.To <= to)
                              .ToList();
    }
}
