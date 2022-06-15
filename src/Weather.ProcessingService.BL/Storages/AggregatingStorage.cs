using System.Collections.Concurrent;
using Weather.ProcessingService.BL.Models;
using Weather.ProcessingService.BL.Storages.Interfaces;

namespace Weather.ProcessingService.BL.Storages;

public class AggregatingStorage : IAggregatingStorage
{
    private readonly ConcurrentDictionary<Guid, List<AggregatedData>> _storage = new();

    private readonly object _locker = new();
    
    public void Add(AggregatedData aggregatedData)
    {
        lock(_locker)
        {
            if(_storage.ContainsKey(aggregatedData.SensorId))
                _storage[aggregatedData.SensorId].Add(aggregatedData);
            else 
                _storage[aggregatedData.SensorId] = new List<AggregatedData> { aggregatedData };
        }
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
        return _storage[sensorId];
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
            dataBySensors[sensorId] = _storage[sensorId].ToArray();
        }
        
        return dataBySensors;
    }

    public IDictionary<Guid, IEnumerable<AggregatedData>> GetAggregatingDataForPeriod(IEnumerable<Guid> sensorIds, Period period)
    {
        var dataBySensors = new Dictionary<Guid, IEnumerable<AggregatedData>>();
        foreach(var sensorId in sensorIds)
        {
            dataBySensors[sensorId] = GetAggregatingDataBySensorInternal(sensorId, period);
        }
        
        return dataBySensors;
    }
    
    private IEnumerable<AggregatedData> GetAggregatingDataBySensorInternal(Guid sensorId, Period period)
    {
        if(period.From.CompareTo(period.To) > 0)
            throw new ArgumentException($"Datetime From ({period.From}) couldn't bu more then datetime To ({period.To})");

        var aggregatingData = _storage[sensorId];

        var from = new DateTime(period.From.Year, period.From.Month, period.From.Day, period.From.Hour, period.From.Minute, 0);
        var to = new DateTime(period.To.Year, period.To.Month, period.To.Day, period.To.Hour, period.To.Minute, 0);

        return aggregatingData.Where(p => from <= p.Period!.From && p.Period.To <= to)
                              .ToList();
    }
}
