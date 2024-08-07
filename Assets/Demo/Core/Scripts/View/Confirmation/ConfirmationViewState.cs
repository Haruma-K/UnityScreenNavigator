using System;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.Confirmation
{
    public sealed class ConfirmationViewState : AppViewState, IConfirmationViewState
    {
        private readonly ReactiveProperty<string> _message = new();
        private readonly Subject<Unit> _onCloseButtonClickedSubject = new();

        public IReactiveProperty<string> Message => _message;
        public IObservable<Unit> CloseButtonClicked => _onCloseButtonClickedSubject;

        void IConfirmationViewState.InvokeCloseButtonClicked()
        {
            _onCloseButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            _onCloseButtonClickedSubject.Dispose();
            _message.Dispose();
        }
    }

    internal interface IConfirmationViewState
    {
        void InvokeCloseButtonClicked();
    }
}