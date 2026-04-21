using System;

namespace SDK.Domain.Common
{
    public interface IObservableStream<out T>
    {
        /// <summary>
        /// Subscribes an observer callback to receive published payloads.
        /// </summary>
        /// <param name="observer">Callback invoked for each payload.</param>
        /// <returns>A disposable subscription handle.</returns>
        IDisposable Subscribe(Action<T> observer);
    }

    public interface IEventStream<T> : IObservableStream<T>
    {
        /// <summary>
        /// Publishes a payload to all current subscribers.
        /// </summary>
        /// <param name="payload">Payload to broadcast.</param>
        void Publish(T payload);
    }
}
