using System.Threading.Tasks;
using Demo.Core.Scripts.Foundation.Common;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.UnitPortraitViewer;
using UniRx;

namespace Demo.Core.Scripts.Presentation.UnitPortraitViewer
{
    public sealed class UnitPortraitViewerModalPresenter
        : ModalPresenterBase<UnitPortraitViewerModal, UnitPortraitViewerView, UnitPortraitViewerViewState>
    {
        private readonly string _unitTypeMasterId;
        private readonly int _unitRank;

        public UnitPortraitViewerModalPresenter(UnitPortraitViewerModal view, ITransitionService transitionService,
            string unitTypeMasterId, int unitRank) : base(view, transitionService)
        {
            _unitTypeMasterId = unitTypeMasterId;
            _unitRank = unitRank;
        }

        protected override Task ViewDidLoad(UnitPortraitViewerModal view, UnitPortraitViewerViewState viewState)
        {
            viewState.Portrait.ImageResourceKey.Value =
                ResourceKey.Textures.GetUnitPortrait(_unitTypeMasterId, _unitRank);
            viewState.OnCloseButtonClicked.Subscribe(_ => TransitionService.PopCommandExecuted());

            return Task.CompletedTask;
        }
    }
}
