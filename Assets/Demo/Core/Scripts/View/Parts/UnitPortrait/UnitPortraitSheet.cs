using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.Parts.UnitPortrait
{
    public sealed class UnitPortraitSheet : Sheet<UnitPortraitView, UnitPortraitViewState>
    {
        protected override ViewInitializationTiming RootInitializationTiming =>
            ViewInitializationTiming.BeforeFirstEnter;
    }
}
