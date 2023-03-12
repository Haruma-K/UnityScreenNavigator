using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Top;

namespace Demo.Core.Scripts.Presentation.Top
{
    public sealed class TopPagePresenterFactory
    {
        public TopPagePresenter Create(TopPage view, ITransitionService transitionService)
        {
            return new TopPagePresenter(view, transitionService);
        }
    }
}