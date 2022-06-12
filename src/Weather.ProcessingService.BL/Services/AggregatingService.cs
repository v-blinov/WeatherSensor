using Weather.ProcessingService.BL.Models;
using Weather.ProcessingService.BL.Models.Dtos;
using Weather.ProcessingService.BL.Services.Interfaces;
using Weather.ProcessingService.BL.Storages.Interfaces;

namespace Weather.ProcessingService.BL.Services;

public class AggregatingService : IAggregatingService
{
    private readonly IAggregatingStorage _aggregatingStorage;

    public AggregatingService(IAggregatingStorage aggregatingStorage)
    {
        _aggregatingStorage = aggregatingStorage;
    }
    
    public void Add(Guid sensorId, IEnumerable<EventDto> eventDtos, PeriodDto periodDto)
    {
        var data = new AggregatedData
        {
            SensorId = sensorId,
            Period = new Period
            {
                From = new DateTime(periodDto.From.Year, periodDto.From.Month, periodDto.From.Day, periodDto.From.Hour, periodDto.From.Minute, 0),
                To = new DateTime(periodDto.To.Year, periodDto.To.Month, periodDto.To.Day, periodDto.To.Hour, periodDto.To.Minute, 0)
            },
            AverageTemperature = eventDtos.Average(e => e.Temperature),
            AverageAirHumidity = eventDtos.Average(e => e.AirHumidity),
            MaxCo2 = eventDtos.Max(e=> e.Co2),
            MinCo2 = eventDtos.Min(e => e.Co2)
        };
        
        _aggregatingStorage.Add(data);
    }
    public Dictionary<Guid, IEnumerable<AggregatedData>> GetAllAggregatedItems()
    {
        var data = _aggregatingStorage.GetAggregatedData();
        
        var dataDtos = new Dictionary<Guid, IEnumerable<AggregatedData>>(data.Count);
        foreach(var item in data)
        {
            dataDtos[item.Key] = data[item.Key].Select(p => new AggregatedData
            {
                SensorId = p.SensorId,
                Period = p.Period,
                AverageTemperature = p.AverageTemperature,
                AverageAirHumidity = p.AverageAirHumidity,
                MaxCo2 = p.MaxCo2,
                MinCo2 = p.MinCo2
            }).ToArray();
        }

        return dataDtos;
    }
    

    public IEnumerable<AggregatedDataDto> GetAggregatedDataForInterval(DateTime from, DateTime to, Guid sensorId)
    {
        return GetAggregatedDataForIntervalInternal(from, to, sensorId);
    }

    public IEnumerable<AggregatedDataDto> GetAggregatedDataForInterval(DateTime from, int durationInMinutes, Guid sensorId)
    {
        var to = from.AddMinutes(durationInMinutes);
        return GetAggregatedDataForIntervalInternal(from, to, sensorId);
    }

    public Dictionary<Guid, IEnumerable<AggregatedDataDto>> GetAggregatedDataForInterval(DateTime from, DateTime to)
    {
        return GetAggregatedDataForIntervalInternal(from, to);
    }

    public Dictionary<Guid, IEnumerable<AggregatedDataDto>> GetAggregatedDataForInterval(DateTime from, int durationInMinutes)
    {
        var to = from.AddMinutes(durationInMinutes);
        return GetAggregatedDataForIntervalInternal(from, to);
    }

    
    private IEnumerable<AggregatedDataDto> GetAggregatedDataForIntervalInternal(DateTime from, DateTime to, Guid sensorId)
    {
        var period = new Period
        {
            From = new(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0),
            To = new(to.Year, to.Month, to.Day, to.Hour, to.Minute, 0)
        };
        
        var aggregating = _aggregatingStorage.GetAggregatedDataBySensorIdForPeriod(sensorId, period);
        return aggregating.Select(p => new AggregatedDataDto
        {
            SensorId = sensorId,
            Period = new PeriodDto { From = period.From, To = period.To, },
            AverageTemperature = p.AverageTemperature,
            AverageAirHumidity = p.AverageAirHumidity,
            MaxCo2 = p.MaxCo2,
            MinCo2 = p.MinCo2
        }).ToArray();
    }
    private Dictionary<Guid, IEnumerable<AggregatedDataDto>> GetAggregatedDataForIntervalInternal(DateTime from, DateTime to)
    {
        var period = new Period
        {
            From = (new DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0)).ToUniversalTime(),
            To = (new DateTime(to.Year, to.Month, to.Day, to.Hour, to.Minute, 0)).ToUniversalTime(),
        };

        var result = new Dictionary<Guid, IEnumerable<AggregatedDataDto>>();
        
        var aggregating = _aggregatingStorage.GetAggregatedDataForPeriod(period);
        foreach(var item in aggregating)
        {
            result[item.Key] = item.Value.Select(p => new AggregatedDataDto
            {
                SensorId = item.Key,
                Period = new PeriodDto { From = period.From, To = period.To, },
                AverageTemperature = p.AverageTemperature,
                AverageAirHumidity = p.AverageAirHumidity,
                MaxCo2 = p.MaxCo2,
                MinCo2 = p.MinCo2
            }).ToArray();
        }

        return result;
    }
}