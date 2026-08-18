using System;
using Demo.Subsystem.PresentationFramework;
using R3;

namespace Demo.Core.Scripts.View.Top
{
    public sealed class TopViewState : AppViewState, ITopState
    {
        private readonly Subject<Unit> _onClickedSubject = new Subject<Unit>();

        public Observable<Unit> OnClicked => _onClickedSubject;

        void ITopState.InvokeBackButtonClicked()
        {
            _onClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            _onClickedSubject.Dispose();
        }
    }

    internal interface ITopState
    {
        void InvokeBackButtonClicked();
    }
}
