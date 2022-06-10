namespace ObserverLibrary.Interfaces;

public interface IPublisher
{
    void Subscribe(string subscriberId);
    void Unsubscribe(string subscriberId);
    void Notify();
}