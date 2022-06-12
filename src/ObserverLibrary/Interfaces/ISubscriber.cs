using ObserverLibrary.Models;

namespace ObserverLibrary.Interfaces;

public interface ISubscriber
{
    // Получаем обновление от издателя
    void UpdateSensorEventsQueue(Event @event);
}