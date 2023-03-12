using System;
using Demo.Core.Scripts.View.Parts.UnitThumbnail;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopItemViewState : AppViewState, IUnitShopItemState
    {
        private readonly ReactiveProperty<int> _cost = new ReactiveProperty<int>();
        private readonly ReactiveProperty<string> _costIconImageResourceKey = new ReactiveProperty<string>();
        private readonly ReactiveProperty<bool> _isLocked = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _isSoldOut = new ReactiveProperty<bool>();
        private readonly Subject<Unit> _onBuyButtonClickedSubject = new Subject<Unit>();
        public UnitThumbnailViewState Thumbnail { get; } = new UnitThumbnailViewState();

        public IReactiveProperty<int> Cost => _cost;
        public IReactiveProperty<string> CostIconImageResourceKey => _costIconImageResourceKey;
        public IReactiveProperty<bool> IsLocked => _isLocked;
        public IReactiveProperty<bool> IsSoldOut => _isSoldOut;
        public IObservable<Unit> OnBuyButtonClicked => _onBuyButtonClickedSubject;

        public void InvokeBuyButtonClicked()
        {
            _onBuyButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            Thumbnail.Dispose();
            _onBuyButtonClickedSubject.Dispose();
            _cost.Dispose();
            _costIconImageResourceKey.Dispose();
            _isLocked.Dispose();
            _isSoldOut.Dispose();
        }
    }

    internal interface IUnitShopItemState
    {
    }
}
