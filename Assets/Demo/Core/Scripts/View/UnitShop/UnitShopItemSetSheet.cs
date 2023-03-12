using Demo.Subsystem.GUIComponents.TabGroup;
using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopItemSetSheet : Sheet<UnitShopItemSetView, UnitShopItemSetViewState>, ITabContent
    {
        protected override ViewInitializationTiming RootInitializationTiming =>
            ViewInitializationTiming.BeforeFirstEnter;

        public int TabIndex { get; private set; }

        void ITabContent.SetTabIndex(int index)
        {
            TabIndex = index;
        }
    }
}
