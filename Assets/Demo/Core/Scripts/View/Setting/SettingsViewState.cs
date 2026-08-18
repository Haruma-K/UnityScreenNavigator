using System;
using Demo.Subsystem.PresentationFramework;
using R3;

namespace Demo.Core.Scripts.View.Setting
{
    public sealed class SettingsViewState : AppViewState, ISettingsState
    {
        private readonly Subject<Unit> _onCloseButtonClickedSubject = new Subject<Unit>();
        private readonly Subject<Unit> _onLockedButtonClickedSubject = new();

        public SoundSettingsViewState SoundSettings { get; } = new SoundSettingsViewState();
        public Observable<Unit> CloseButtonClicked => _onCloseButtonClickedSubject;
        public Observable<Unit> LockedButtonClicked => _onLockedButtonClickedSubject;

        void ISettingsState.InvokeCloseButtonClicked()
        {
            _onCloseButtonClickedSubject.OnNext(Unit.Default);
        }

        void ISettingsState.InvokeLockedButtonClicked()
        {
            _onLockedButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            SoundSettings.Dispose();
            _onCloseButtonClickedSubject.Dispose();
            _onLockedButtonClickedSubject.Dispose();
        }
    }

    internal interface ISettingsState
    {
        void InvokeCloseButtonClicked();

        void InvokeLockedButtonClicked();
    }
}
