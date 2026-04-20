using System;

namespace SDK.Domain.Common
{
    public interface IObservableStream<out T>
    {
        IDisposable Subscribe(Action<T> observer);
    }

    public interface IEventStream<T> : IObservableStream<T>
    {
        void Publish(T payload);
    }
}
