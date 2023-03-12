using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Loading;

namespace Demo.Core.Scripts.Presentation.Loading
{
    public sealed class LoadingPagePresenter : PagePresenterBase<LoadingPage, LoadingView, LoadingViewState>
    {
        public LoadingPagePresenter(LoadingPage view, ITransitionService transitionService)
            : base(view, transitionService)
        {
        }

        protected override void ViewDidPushEnter(LoadingPage view, LoadingViewState viewState)
        {
            TransitionService.HomeLoadingPageShown();
        }
    }
}
