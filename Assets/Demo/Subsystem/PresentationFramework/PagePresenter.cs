using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Subsystem.Misc;
using Demo.Subsystem.PresentationFramework.UnityScreenNavigatorExtensions;

namespace Demo.Subsystem.PresentationFramework
{
    public abstract class PagePresenter<TPage, TRootView, TRootViewState> : PagePresenter<TPage>,
        IDisposableCollectionHolder
        where TPage : Page<TRootView, TRootViewState>
        where TRootView : AppView<TRootViewState>
        where TRootViewState : AppViewState, new()
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private TRootViewState _state;

        protected PagePresenter(TPage view) : base(view)
        {
        }

        ICollection<IDisposable> IDisposableCollectionHolder.GetDisposableCollection()
        {
            return _disposables;
        }

        protected sealed override void Initialize(TPage view)
        {
            base.Initialize(view);
        }

        protected sealed override async Task ViewDidLoad(TPage view)
        {
            await base.ViewDidLoad(view);
            var state = new TRootViewState();
            _state = state;
            _disposables.Add(state);
            view.Setup(state);
            await ViewDidLoad(view, _state);
        }

        protected sealed override async Task ViewWillPushEnter(TPage view)
        {
            await base.ViewWillPushEnter(view);
            await ViewWillPushEnter(view, _state);
        }

        protected sealed override void ViewDidPushEnter(TPage view)
        {
            base.ViewDidPushEnter(view);
            ViewDidPushEnter(view, _state);
        }

        protected sealed override async Task ViewWillPushExit(TPage view)
        {
            await base.ViewWillPushExit(view);
            await ViewWillPushExit(view, _state);
        }

        protected sealed override void ViewDidPushExit(TPage view)
        {
            base.ViewDidPushExit(view);
            ViewDidPushExit(view, _state);
        }

        protected sealed override async Task ViewWillPopEnter(TPage view)
        {
            await base.ViewWillPopEnter(view);
            await ViewWillPopEnter(view, _state);
        }

        protected sealed override void ViewDidPopEnter(TPage view)
        {
            base.ViewDidPopEnter(view);
            ViewDidPopEnter(view, _state);
        }

        protected sealed override async Task ViewWillPopExit(TPage view)
        {
            await base.ViewWillPopExit(view);
            await ViewWillPopExit(view, _state);
        }

        protected sealed override void ViewDidPopExit(TPage view)
        {
            base.ViewDidPopExit(view);
            ViewDidPopExit(view, _state);
        }

        protected override async Task ViewWillDestroy(TPage view)
        {
            await base.ViewWillDestroy(view);
            await ViewWillDestroy(view, _state);
        }

        protected virtual Task ViewDidLoad(TPage view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual Task ViewWillPushEnter(TPage view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual void ViewDidPushEnter(TPage view, TRootViewState viewState)
        {
        }

        protected virtual Task ViewWillPushExit(TPage view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual void ViewDidPushExit(TPage view, TRootViewState viewState)
        {
        }

        protected virtual Task ViewWillPopEnter(TPage view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual void ViewDidPopEnter(TPage view, TRootViewState viewState)
        {
        }

        protected virtual Task ViewWillPopExit(TPage view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected virtual void ViewDidPopExit(TPage view, TRootViewState viewState)
        {
        }

        protected virtual Task ViewWillDestroy(TPage view, TRootViewState viewState)
        {
            return Task.CompletedTask;
        }

        protected sealed override void Dispose(TPage view)
        {
            base.Dispose(view);
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
