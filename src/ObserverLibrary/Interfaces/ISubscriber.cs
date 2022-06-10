using ObserverLibrary.Models;

namespace ObserverLibrary.Interfaces;

public interface ISubscriber
{
    public Guid Id { get; init; }
    // Получаем обновление от издателя
    void Update(Event publisher);
}