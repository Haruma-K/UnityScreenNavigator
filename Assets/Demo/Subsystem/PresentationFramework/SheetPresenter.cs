using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Subsystem.Misc;
using Demo.Subsystem.PresentationFramework.UnityScreenNavigatorExtensions;

namespace Demo.Subsystem.PresentationFramework
{
    public abstract class SheetPresenter<TSheet, TRootView, TRootViewState> : SheetPresenter<TSheet>,
        IDisposableCollectionHolder
        where TSheet : Sheet<TRootView, TRootViewState>
        where TRootView : AppView<TRootViewState>
        where TRootViewState : AppViewState, new()
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        private TRootViewState _state;

        protected SheetPresenter(TSheet view) : base(view)
        {
        }

        ICollection<IDisposable> IDisposableCollectionHolder.GetDisposableCollection()
        {
            return _disposables;
        }

        protected sealed override void Initialize(TSheet view)
        {
            base.Initialize(view);
        }

        protected sealed override async Task ViewDidLoad(TSheet view)
        {
            await base.ViewDidLoad(view);
            var state = new TRootViewState();
            _state = state;
            _disposables.Add(state);
            view.Setup(state);
            await ViewDidLoad(view, _state);
        }

        protected sealed override async Task ViewWillEnter(TSheet view)
        {
            await base.ViewWillEnter(view);
            await ViewWillEnter(view, _state);
        }

        protected sealed override void ViewDidEnter(TSheet view)
        {
            base.ViewDidEnter(view);
            ViewDidEnter(view, _state);
        }

        protected sealed override async Task ViewWillExit(TSheet view)
        {
            await base.ViewWillExit(view);
            await ViewWillExit(view, _state);
        }

        protected sealed override void ViewDidExit(TSheet view)
        {
            base.ViewDidExit(view);
            ViewDidExit(view, _state);
        }

        protected override async Task ViewWillDestroy(TSheet view)
        {
            await base.ViewWillDestroy(view);
            await ViewWillDestroy(view, _state);
        }

        protected virtual Task ViewDidLoad(TSheet view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ViewWillEnter(TSheet view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual void ViewDidEnter(TSheet view, TRootViewState viewState)
        {
        }

        protected virtual Task ViewWillExit(TSheet view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual void ViewDidExit(TSheet view, TRootViewState viewState)
        {
        }

        protected virtual Task ViewWillDestroy(TSheet view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected sealed override void Dispose(TSheet view)
        {
            base.Dispose(view);
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
