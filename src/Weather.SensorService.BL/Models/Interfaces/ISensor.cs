using ObserverLibrary.Interfaces;

namespace Weather.SensorService.BL.Models.Interfaces;

public interface ISensor : IPublisher
{
    public Guid Id { get; init; }
    public Event State { get; init; }
    public SensorSettings SensorSettings { get; init; }
    public Event GenerateEvent();
}
