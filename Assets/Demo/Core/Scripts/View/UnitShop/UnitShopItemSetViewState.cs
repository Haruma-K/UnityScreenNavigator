using Demo.Core.Scripts.View.Foundation;
using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopItemSetViewState : AppViewState, IUnitShopItemSetState
    {
        public UnitShopItemViewState Item1 { get; } = new UnitShopItemViewState();
        public UnitShopItemViewState Item2 { get; } = new UnitShopItemViewState();
        public UnitShopItemViewState Item3 { get; } = new UnitShopItemViewState();
        public UnitShopItemViewState Item4 { get; } = new UnitShopItemViewState();
        public UnitShopItemViewState Item5 { get; } = new UnitShopItemViewState();
        public UnitShopItemViewState Item6 { get; } = new UnitShopItemViewState();
        public UnitShopItemViewState Item7 { get; } = new UnitShopItemViewState();
        public UnitShopItemViewState Item8 { get; } = new UnitShopItemViewState();

        protected override void DisposeInternal()
        {
            Item1.Dispose();
            Item2.Dispose();
            Item3.Dispose();
            Item4.Dispose();
            Item5.Dispose();
            Item6.Dispose();
            Item7.Dispose();
            Item8.Dispose();
        }
    }

    internal interface IUnitShopItemSetState
    {
    }
}