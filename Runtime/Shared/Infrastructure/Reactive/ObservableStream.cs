using System;
using System.Collections.Generic;
using SDK.Domain.Common;

namespace SDK.Infrastructure.Reactive
{
    public sealed class ObservableStream<T> : IEventStream<T>
    {
        private readonly List<Action<T>> _observers = new List<Action<T>>();

        /// <summary>
        /// Adds an observer that will receive future payloads.
        /// </summary>
        /// <param name="observer">Observer callback.</param>
        /// <returns>A disposable subscription.</returns>
        public IDisposable Subscribe(Action<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            _observers.Add(observer);
            return new Subscription(this, observer);
        }

        /// <summary>
        /// Broadcasts a payload to all active observers.
        /// </summary>
        /// <param name="payload">Payload to broadcast.</param>
        public void Publish(T payload)
        {
            for (var i = 0; i < _observers.Count; i++)
            {
                _observers[i].Invoke(payload);
            }
        }

        private void Unsubscribe(Action<T> observer)
        {
            _observers.Remove(observer);
        }

        private sealed class Subscription : IDisposable
        {
            private readonly ObservableStream<T> _owner;
            private readonly Action<T> _observer;
            private bool _disposed;

            public Subscription(ObservableStream<T> owner, Action<T> observer)
            {
                _owner = owner;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;
                _owner.Unsubscribe(_observer);
            }
        }
    }
}
