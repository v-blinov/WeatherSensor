using ObserverLibrary.Models;

namespace ObserverLibrary.Interfaces;

public interface IPublisher
{
    void Subscribe(ISubscriber subscriber);
    void Unsubscribe(ISubscriber subscriber);
    void Notify(Event @event);
}