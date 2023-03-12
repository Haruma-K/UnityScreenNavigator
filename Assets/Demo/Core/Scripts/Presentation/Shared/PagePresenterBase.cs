using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.Presentation.Shared
{
    public abstract class PagePresenterBase<TPage, TRootView, TRootViewState>
        : PagePresenter<TPage, TRootView, TRootViewState>
        where TPage : Page<TRootView, TRootViewState>
        where TRootView : AppView<TRootViewState>
        where TRootViewState : AppViewState, new()
    {
        protected PagePresenterBase(TPage view, ITransitionService transitionService) : base(view)
        {
            TransitionService = transitionService;
        }

        protected ITransitionService TransitionService { get; }
    }
}
