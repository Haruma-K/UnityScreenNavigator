namespace Demo.Core.Scripts.Presentation.Shared
{
    public interface ITransitionService
    {
        void ApplicationStarted();

        void TopPageClicked();

        void HomeLoadingPageShown();

        void HomePageUnitShopButtonClicked();

        void HomePageSettingsButtonClicked();

        void UnitShopItemClicked(string unitTypeMasterId);

        void UnitTypeInformationExpandButtonClicked(string unitTypeMasterId, int unitRank);

        void PopCommandExecuted();

        void SettingsModalLockedButtonClicked();
    }
}
