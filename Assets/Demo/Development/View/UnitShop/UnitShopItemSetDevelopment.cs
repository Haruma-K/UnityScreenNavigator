using Demo.Core.Scripts.Foundation.Common;
using Demo.Core.Scripts.View.UnitShop;
using Demo.Development.View.Shared;
using UniRx;
using UnityEngine;

namespace Demo.Development.View.UnitShop
{
    public sealed class UnitShopItemSetDevelopment : AppViewDevelopment<UnitShopItemSetView, UnitShopItemSetViewState>
    {
        protected override UnitShopItemSetViewState CreateState()
        {
            var state = new UnitShopItemSetViewState();

            state.Item1.IsLocked.Value = false;
            state.Item1.Cost.Value = 5000;
            state.Item1.CostIconImageResourceKey.Value = ResourceKey.Textures.CoinIcon;
            state.Item1.Thumbnail.ImageResourceKey.Value = ResourceKey.Textures.GetUnitThumbnail("003", 1);
            state.Item1.Thumbnail.OnClicked.Subscribe(_ => { Debug.Log("Clicked"); }).AddTo(this);
            state.Item1.IsSoldOut.Value = false;
            state.Item1.OnBuyButtonClicked.Subscribe(_ => { Debug.Log("Buy"); }).AddTo(this);

            state.Item2.IsLocked.Value = false;
            state.Item2.Cost.Value = 10000;
            state.Item2.CostIconImageResourceKey.Value = ResourceKey.Textures.CoinIcon;
            state.Item2.Thumbnail.ImageResourceKey.Value = ResourceKey.Textures.GetUnitThumbnail("003", 2);
            state.Item2.Thumbnail.OnClicked.Subscribe(_ => { Debug.Log("Clicked"); }).AddTo(this);
            state.Item2.IsSoldOut.Value = true;
            state.Item2.OnBuyButtonClicked.Subscribe(_ => { Debug.Log("Buy"); }).AddTo(this);

            state.Item3.IsLocked.Value = true;
            state.Item4.IsLocked.Value = true;
            state.Item5.IsLocked.Value = true;
            state.Item6.IsLocked.Value = true;
            state.Item7.IsLocked.Value = true;
            state.Item8.IsLocked.Value = true;
            return state;
        }
    }
}
