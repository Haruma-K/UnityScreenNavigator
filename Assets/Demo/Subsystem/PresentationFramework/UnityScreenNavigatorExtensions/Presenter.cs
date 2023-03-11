using System;

namespace Demo.Subsystem.PresentationFramework.UnityScreenNavigatorExtensions
{
    public abstract class Presenter<TView> : IDisposable
    {
        protected Presenter(TView view)
        {
            View = view;
        }

        public bool IsDisposed { get; private set; }

        public bool IsInitialized { get; private set; }

        private TView View { get; }

        public virtual void Dispose()
        {
            if (!IsInitialized)
                return;

            if (IsDisposed)
                return;

            Dispose(View);
            IsDisposed = true;
        }

        public void Initialize()
        {
            if (IsInitialized)
                throw new InvalidOperationException($"{GetType().Name} is already initialized.");

            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Presenter<TView>));

            Initialize(View);
            IsInitialized = true;
        }

        /// <summary>
        ///     Initializes the presenter.
        /// </summary>
        /// <param name="view"></param>
        protected abstract void Initialize(TView view);

        /// <summary>
        ///     Disposes the presenter.
        /// </summary>
        /// <param name="view"></param>
        protected abstract void Dispose(TView view);
    }

    public abstract class Presenter<TView, TDataSource> : IDisposable
    {
        protected Presenter(TView view, TDataSource dataSource)
        {
            View = view;
            DataSource = dataSource;
        }

        public bool IsDisposed { get; private set; }

        public bool IsInitialized { get; private set; }

        private TView View { get; }
        private TDataSource DataSource { get; }

        public virtual void Dispose()
        {
            if (!IsInitialized)
                return;

            if (IsDisposed)
                return;

            Dispose(View, DataSource);
            IsDisposed = true;
        }

        public void Initialize()
        {
            if (IsInitialized)
                throw new InvalidOperationException($"{GetType().Name} is already initialized.");

            if (IsDisposed)
                throw new ObjectDisposedException(nameof(Presenter<TView, TDataSource>));

            Initialize(View, DataSource);
            IsInitialized = true;
        }

        /// <summary>
        ///     Initializes the presenter.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="dataStore"></param>
        protected abstract void Initialize(TView view, TDataSource dataStore);

        /// <summary>
        ///     Disposes the presenter.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="dataSource"></param>
        protected abstract void Dispose(TView view, TDataSource dataSource);
    }
}
