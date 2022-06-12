using ObserverLibrary.Interfaces;
using Weather.SensorService.BL.Models.Dtos;

namespace Weather.SensorService.BL.Services.Interfaces;

public interface ISensorService
{
    public void Add(SensorDto sensorDto);
    public void AddRange(IEnumerable<SensorDto> sensorDtos);
    public SensorDto GetSensor(Guid id);
    public IEnumerable<SensorDto> GetSensors();
    public IEnumerable<SensorDto> GetSensors(IEnumerable<Guid> ids);
    public IEnumerable<Guid> GetSensorIds();

    public void TrySubscribe(ISubscriber subscriber, Guid sensorId);
    public void TryUnsubscribe(ISubscriber subscriber, Guid sensorId);
}