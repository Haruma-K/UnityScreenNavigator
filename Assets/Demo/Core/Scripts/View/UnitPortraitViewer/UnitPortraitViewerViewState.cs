using System;
using Demo.Core.Scripts.View.Foundation;
using Demo.Core.Scripts.View.Parts.UnitPortrait;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.UnitPortraitViewer
{
    public sealed class UnitPortraitViewerViewState : AppViewState, IUnitPortraitViewerState
    {
        private readonly Subject<Unit> _onCloseButtonClickedSubject = new Subject<Unit>();

        public UnitPortraitViewState Portrait { get; } = new UnitPortraitViewState();
        public IObservable<Unit> OnCloseButtonClicked => _onCloseButtonClickedSubject;

        public void InvokeOnCloseButtonClicked()
        {
            _onCloseButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            Portrait.Dispose();
            _onCloseButtonClickedSubject.Dispose();
        }
    }

    internal interface IUnitPortraitViewerState
    {
        void InvokeOnCloseButtonClicked();
    }
}
