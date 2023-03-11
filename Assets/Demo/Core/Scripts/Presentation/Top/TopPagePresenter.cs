using System.Threading.Tasks;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Top;
using Demo.Subsystem.Misc;
using UniRx;

namespace Demo.Core.Scripts.Presentation.Top
{
    public sealed class TopPagePresenter : PagePresenterBase<TopPage, TopView, TopViewState>
    {
        public TopPagePresenter(TopPage view, ITransitionService transitionService) : base(view, transitionService)
        {
        }

        protected override Task ViewDidLoad(TopPage view, TopViewState viewState)
        {
            viewState.OnClicked.Subscribe(_ => TransitionService.TopPageClicked()).AddTo(this);

            return Task.CompletedTask;
        }
    }
}
