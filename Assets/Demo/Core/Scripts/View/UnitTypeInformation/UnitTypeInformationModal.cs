using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.UnitTypeInformation
{
    public sealed class UnitTypeInformationModal : Modal<UnitTypeInformationView, UnitTypeInformationViewState>
    {
        protected override ViewInitializationTiming RootInitializationTiming =>
            ViewInitializationTiming.BeforeFirstEnter;
    }
}
