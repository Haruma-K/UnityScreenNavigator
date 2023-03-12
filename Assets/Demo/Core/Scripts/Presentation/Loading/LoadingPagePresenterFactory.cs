using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Loading;

namespace Demo.Core.Scripts.Presentation.Loading
{
    public sealed class LoadingPagePresenterFactory
    {
        public LoadingPagePresenter Create(LoadingPage view, ITransitionService transitionService)
        {
            return new LoadingPagePresenter(view, transitionService);
        }
    }
}
