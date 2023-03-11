using System;
using Demo.Core.Scripts.View.Foundation;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.Setting
{
    public sealed class SettingsViewState : AppViewState, ISettingsState
    {
        private readonly Subject<Unit> _onCloseButtonClickedSubject = new Subject<Unit>();

        public SoundSettingsViewState SoundSettings { get; } = new SoundSettingsViewState();
        public IObservable<Unit> CloseButtonClicked => _onCloseButtonClickedSubject;

        void ISettingsState.InvokeCloseButtonClicked()
        {
            _onCloseButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            SoundSettings.Dispose();
            _onCloseButtonClickedSubject.Dispose();
        }
    }

    internal interface ISettingsState
    {
        void InvokeCloseButtonClicked();
    }
}
