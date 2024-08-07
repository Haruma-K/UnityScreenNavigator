using System;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.Setting
{
    public sealed class SettingsViewState : AppViewState, ISettingsState
    {
        private readonly Subject<Unit> _onCloseButtonClickedSubject = new Subject<Unit>();
        private readonly Subject<Unit> _onLockedButtonClickedSubject = new();

        public SoundSettingsViewState SoundSettings { get; } = new SoundSettingsViewState();
        public IObservable<Unit> CloseButtonClicked => _onCloseButtonClickedSubject;
        public IObservable<Unit> LockedButtonClicked => _onLockedButtonClickedSubject;

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
