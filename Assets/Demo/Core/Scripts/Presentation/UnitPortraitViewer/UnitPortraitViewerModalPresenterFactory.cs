using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.UnitPortraitViewer;

namespace Demo.Core.Scripts.Presentation.UnitPortraitViewer
{
    public sealed class UnitPortraitViewerModalPresenterFactory
    {
        public UnitPortraitViewerModalPresenter Create(UnitPortraitViewerModal view,
            ITransitionService transitionService, string unitTypeMasterId, int unitRank)
        {
            return new UnitPortraitViewerModalPresenter(view, transitionService, unitTypeMasterId, unitRank);
        }
    }
}
