using System;
using Demo.Core.Scripts.View.Foundation;
using Demo.Core.Scripts.View.Parts.UnitPortrait;
using Demo.Core.Scripts.View.Parts.UnitThumbnail;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.UnitTypeInformation
{
    public sealed class UnitTypeInformationViewState : AppViewState, IUnitTypeInformationState
    {
        private readonly Subject<Unit> _onCloseButtonClickedSubject = new Subject<Unit>();
        private readonly Subject<Unit> _onExpandButtonClickedSubject = new Subject<Unit>();
        private readonly ReactiveProperty<string> _rank1Description = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> _rank2Description = new ReactiveProperty<string>();
        private readonly ReactiveProperty<string> _rank3Description = new ReactiveProperty<string>();
        private readonly ReactiveProperty<int> _tabIndex = new ReactiveProperty<int>();
        private readonly ReactiveProperty<string> _title = new ReactiveProperty<string>();

        public IReactiveProperty<string> Title => _title;
        public IReactiveProperty<int> TabIndex => _tabIndex;
        public IReactiveProperty<string> Rank1Description => _rank1Description;
        public IReactiveProperty<string> Rank2Description => _rank2Description;
        public IReactiveProperty<string> Rank3Description => _rank3Description;
        public UnitThumbnailViewState Rank1Thumbnail { get; } = new UnitThumbnailViewState();
        public UnitThumbnailViewState Rank2Thumbnail { get; } = new UnitThumbnailViewState();
        public UnitThumbnailViewState Rank3Thumbnail { get; } = new UnitThumbnailViewState();
        public UnitPortraitViewState Rank1Portrait { get; } = new UnitPortraitViewState();
        public UnitPortraitViewState Rank2Portrait { get; } = new UnitPortraitViewState();
        public UnitPortraitViewState Rank3Portrait { get; } = new UnitPortraitViewState();
        public IObservable<Unit> OnCloseButtonClicked => _onCloseButtonClickedSubject;
        public IObservable<Unit> OnExpandButtonClicked => _onExpandButtonClickedSubject;

        void IUnitTypeInformationState.InvokeCloseButtonClicked()
        {
            _onCloseButtonClickedSubject.OnNext(Unit.Default);
        }

        void IUnitTypeInformationState.InvokeExpandButtonClicked()
        {
            _onExpandButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            _title.Dispose();
            _tabIndex.Dispose();
            _rank1Description.Dispose();
            _rank2Description.Dispose();
            _rank3Description.Dispose();
            Rank1Thumbnail.Dispose();
            Rank2Thumbnail.Dispose();
            Rank3Thumbnail.Dispose();
            Rank1Portrait.Dispose();
            Rank2Portrait.Dispose();
            Rank3Portrait.Dispose();
            _onCloseButtonClickedSubject.Dispose();
            _onExpandButtonClickedSubject.Dispose();
        }
    }

    internal interface IUnitTypeInformationState
    {
        void InvokeCloseButtonClicked();
        void InvokeExpandButtonClicked();
    }
}
