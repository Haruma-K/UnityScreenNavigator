using System;
using Demo.Core.Scripts.View.Foundation;
using Demo.Subsystem.PresentationFramework;
using R3;

namespace Demo.Core.Scripts.View.Home
{
    public sealed class HomeButtonViewState : AppViewState, IHomeButtonState
    {
        private readonly ReactiveProperty<bool> _isLocked = new ReactiveProperty<bool>();
        private readonly Subject<Unit> _onClickedSubject = new Subject<Unit>();

        public ReactiveProperty<bool> IsLocked => _isLocked;
        public Observable<Unit> OnClicked => _onClickedSubject;

        void IHomeButtonState.InvokeClicked()
        {
            _onClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            _onClickedSubject.Dispose();
        }
    }

    internal interface IHomeButtonState
    {
        void InvokeClicked();
    }
}
