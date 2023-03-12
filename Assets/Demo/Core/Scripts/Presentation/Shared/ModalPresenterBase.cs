using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.Presentation.Shared
{
    public abstract class ModalPresenterBase<TModal, TRootView, TRootViewState>
        : ModalPresenter<TModal, TRootView, TRootViewState>
        where TModal : Modal<TRootView, TRootViewState>
        where TRootView : AppView<TRootViewState>
        where TRootViewState : AppViewState, new()
    {
        protected ModalPresenterBase(TModal view, ITransitionService transitionService) : base(view)
        {
            TransitionService = transitionService;
        }

        protected ITransitionService TransitionService { get; }
    }
}
