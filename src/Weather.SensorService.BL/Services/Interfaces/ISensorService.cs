using ObserverLibrary.Interfaces;
using Weather.SensorService.BL.Models.Dtos;
using Weather.SensorService.BL.Models.Interfaces;

namespace Weather.SensorService.BL.Services.Interfaces;

public interface ISensorService
{
    public void Add(ISensor sensor);
    public void AddRange(IEnumerable<ISensor> sensors);
    public SensorDto GetSensor(Guid id);
    public IEnumerable<SensorDto> GetSensors();

    public void TrySubscribe(ISubscriber subscriber, Guid sensorId);
    public void TryUnsubscribe(ISubscriber subscriber, Guid sensorId);
}