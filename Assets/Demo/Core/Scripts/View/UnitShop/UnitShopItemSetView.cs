using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation;
using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopItemSetView : AppView<UnitShopItemSetViewState>
    {
        public UnitShopItemView item1;
        public UnitShopItemView item2;
        public UnitShopItemView item3;
        public UnitShopItemView item4;
        public UnitShopItemView item5;
        public UnitShopItemView item6;
        public UnitShopItemView item7;
        public UnitShopItemView item8;

        protected override async UniTask Initialize(UnitShopItemSetViewState viewState)
        {
            var tasks = new List<UniTask>
            {
                item1.InitializeAsync(viewState.Item1),
                item2.InitializeAsync(viewState.Item2),
                item3.InitializeAsync(viewState.Item3),
                item4.InitializeAsync(viewState.Item4),
                item5.InitializeAsync(viewState.Item5),
                item6.InitializeAsync(viewState.Item6),
                item7.InitializeAsync(viewState.Item7),
                item8.InitializeAsync(viewState.Item8)
            };
            await UniTask.WhenAll(tasks);
        }
    }
}
