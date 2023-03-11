using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.UnitPortraitViewer
{
    public sealed class UnitPortraitViewerModal : Modal<UnitPortraitViewerView, UnitPortraitViewerViewState>
    {
        protected override ViewInitializationTiming RootInitializationTiming =>
            ViewInitializationTiming.BeforeFirstEnter;
    }
}
