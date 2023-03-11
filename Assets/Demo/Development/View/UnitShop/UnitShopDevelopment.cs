using Demo.Core.Scripts.Foundation.Common;
using Demo.Core.Scripts.View.UnitShop;
using Demo.Development.View.Shared;
using UniRx;
using UnityEngine;

namespace Demo.Development.View.UnitShop
{
    public sealed class UnitShopDevelopment : AppViewDevelopment<UnitShopView, UnitShopViewState>
    {
        protected override UnitShopViewState CreateState()
        {
            var state = new UnitShopViewState();
            SetupItemSet(state.RegularItems, 5000, "003", 1);
            SetupItemSet(state.SpecialItems, 10000, "004", 1);
            SetupItemSet(state.SaleItems, 800, "005", 1);
            state.OnBackButtonClicked.Subscribe(_ => Debug.Log("OnBackButtonClicked")).AddTo(this);
            return state;
        }

        private void SetupItemSet(UnitShopItemSetViewState viewState, int cost, string unitTypeMasterId, int unitRank)
        {
            viewState.Item1.IsLocked.Value = false;
            viewState.Item1.Cost.Value = cost;
            viewState.Item1.CostIconImageResourceKey.Value = ResourceKey.Textures.CoinIcon;
            viewState.Item1.Thumbnail.ImageResourceKey.Value =
                ResourceKey.Textures.GetUnitThumbnail(unitTypeMasterId, unitRank);
            viewState.Item1.Thumbnail.OnClicked.Subscribe(_ => { Debug.Log("Clicked"); }).AddTo(this);
            viewState.Item2.IsLocked.Value = true;
            viewState.Item3.IsLocked.Value = true;
            viewState.Item4.IsLocked.Value = true;
            viewState.Item5.IsLocked.Value = true;
            viewState.Item6.IsLocked.Value = true;
            viewState.Item7.IsLocked.Value = true;
            viewState.Item8.IsLocked.Value = true;
        }
    }
}
