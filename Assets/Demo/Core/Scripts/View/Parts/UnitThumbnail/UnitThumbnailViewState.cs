using System;
using Demo.Subsystem.PresentationFramework;
using R3;

namespace Demo.Core.Scripts.View.Parts.UnitThumbnail
{
    public sealed class UnitThumbnailViewState : AppViewState, IUnitThumbnailState
    {
        private readonly ReactiveProperty<string> _imageResourceKey = new ReactiveProperty<string>();
        private readonly Subject<Unit> _onClickedSubject = new Subject<Unit>();

        public ReactiveProperty<string> ImageResourceKey => _imageResourceKey;
        public Observable<Unit> OnClicked => _onClickedSubject;

        void IUnitThumbnailState.InvokeClicked()
        {
            _onClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            _imageResourceKey.Dispose();
            _onClickedSubject.Dispose();
        }
    }

    internal interface IUnitThumbnailState
    {
        void InvokeClicked();
    }
}
