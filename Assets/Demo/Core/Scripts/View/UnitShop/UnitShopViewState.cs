using System;
using Demo.Core.Scripts.View.Foundation;
using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopViewState : AppViewState, IUnitShopState
    {
        private readonly Subject<Unit> _onBackButtonClickedSubject = new Subject<Unit>();
        public UnitShopItemSetViewState RegularItems { get; } = new UnitShopItemSetViewState();
        public UnitShopItemSetViewState SpecialItems { get; } = new UnitShopItemSetViewState();
        public UnitShopItemSetViewState SaleItems { get; } = new UnitShopItemSetViewState();

        public IObservable<Unit> OnBackButtonClicked => _onBackButtonClickedSubject;

        public void InvokeBackButtonClicked()
        {
            _onBackButtonClickedSubject.OnNext(Unit.Default);
        }

        protected override void DisposeInternal()
        {
            _onBackButtonClickedSubject.Dispose();
            RegularItems.Dispose();
            SpecialItems.Dispose();
            SaleItems.Dispose();
        }
    }

    internal interface IUnitShopState
    {
        void InvokeBackButtonClicked();
    }
}
