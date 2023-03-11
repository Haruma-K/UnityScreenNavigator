using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.Presentation.Shared
{
    public abstract class SheetPresenterBase<TSheet, TRootView, TRootViewState>
        : SheetPresenter<TSheet, TRootView, TRootViewState>
        where TSheet : Sheet<TRootView, TRootViewState>
        where TRootView : AppView<TRootViewState>
        where TRootViewState : AppViewState, new()
    {
        protected SheetPresenterBase(TSheet view, ITransitionService transitionService) : base(view)
        {
            TransitionService = transitionService;
        }

        protected ITransitionService TransitionService { get; }
    }
}
