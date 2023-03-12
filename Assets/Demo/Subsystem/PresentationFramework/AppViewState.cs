using System;

namespace Demo.Subsystem.PresentationFramework
{
    public abstract class AppViewState : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(AppViewState));

            DisposeInternal();

            _isDisposed = true;
        }

        protected abstract void DisposeInternal();
    }
}
