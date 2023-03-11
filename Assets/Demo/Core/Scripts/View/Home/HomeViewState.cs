using System;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.Home
{
    public sealed class HomeViewState : AppViewState, IHomeState
    {
        private readonly Subject<Unit> _onBackButtonClickedSubject = new Subject<Unit>();

        public HomeButtonViewState SettingsButton { get; } = new HomeButtonViewState();
        public HomeButtonViewState UnitShopButton { get; } = new HomeButtonViewState();
        public HomeButtonViewState MainQuestButton { get; } = new HomeButtonViewState();
        public HomeButtonViewState MissionButton { get; } = new HomeButtonViewState();
        public HomeButtonViewState EventQuestButton { get; } = new HomeButtonViewState();
        public IObservable<Unit> OnBackButtonClicked => _onBackButtonClickedSubject;

        void IHomeState.InvokeBackButtonClicked()
        {
            _onBackButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            SettingsButton.Dispose();
            UnitShopButton.Dispose();
            MainQuestButton.Dispose();
            MissionButton.Dispose();
            EventQuestButton.Dispose();
            _onBackButtonClickedSubject.Dispose();
        }
    }

    internal interface IHomeState
    {
        void InvokeBackButtonClicked();
    }
}
