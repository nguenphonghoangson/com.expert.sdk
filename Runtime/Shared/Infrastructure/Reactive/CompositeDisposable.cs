using System;
using System.Collections.Generic;

namespace SDK.Infrastructure.Reactive
{
    public sealed class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private bool _disposed;

        public void Add(IDisposable disposable)
        {
            if (_disposed || disposable == null)
            {
                return;
            }

            _disposables.Add(disposable);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            for (var i = 0; i < _disposables.Count; i++)
            {
                _disposables[i].Dispose();
            }

            _disposables.Clear();
        }
    }
}
