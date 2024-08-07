using Demo.Core.Scripts.APIGateway.Setting;
using Demo.Core.Scripts.APIGateway.UnitShop;
using Demo.Core.Scripts.Domain.Setting.Model;
using Demo.Core.Scripts.Domain.UnitShop.Model;
using Demo.Core.Scripts.MasterRepository.FeatureFlag;
using Demo.Core.Scripts.MasterRepository.Unit;
using Demo.Core.Scripts.MasterRepository.UnitShop;
using Demo.Core.Scripts.Presentation.Home;
using Demo.Core.Scripts.Presentation.Loading;
using Demo.Core.Scripts.Presentation.LockConfirmation;
using Demo.Core.Scripts.Presentation.Setting;
using Demo.Core.Scripts.Presentation.Top;
using Demo.Core.Scripts.Presentation.UnitPortraitViewer;
using Demo.Core.Scripts.Presentation.UnitShop;
using Demo.Core.Scripts.Presentation.UnitTypeInformation;
using Demo.Core.Scripts.UseCase.Setting;
using Demo.Core.Scripts.UseCase.UnitShop;
using Demo.Core.Scripts.View.Overlay;
using UnityEngine;

namespace Demo.Core.Scripts.Composition
{
    public sealed class ApplicationCompositionRoot : MonoBehaviour
    {
        public ConnectingView connectingView;

        private void Start()
        {
            Application.targetFrameRate = 60;

            // Models
            var settings = new Settings();
            var unitShopItemSet = new UnitShopItemSet();

            // API Gateways
            var settingsAPIGateway = new SettingsAPIGateway();
            var unitShopAPIGateway = new UnitShopAPIGateway();

            // Master Repositories
            var featureFlagMasterRepository = new FeatureFlagMasterRepository();
            var unitShopMasterRepository = new UnitShopMasterRepository();
            var unitMasterRepository = new UnitMasterRepository();

            // Use Cases
            var settingsService = new SettingsUseCase(settings, settingsAPIGateway);
            var unitShopService = new UnitShopUseCase(unitShopItemSet, unitShopAPIGateway);

            // Presenter Factories
            var homePagePresenterFactory = new HomePagePresenterFactory(featureFlagMasterRepository);
            var loadingPagePresenterFactory = new LoadingPagePresenterFactory();
            var settingsModalPresenterFactory = new SettingsModalPresenterFactory(settingsService);
            var topPagePresenterFactory = new TopPagePresenterFactory();
            var unitPortraitViewerModalPresenterFactory = new UnitPortraitViewerModalPresenterFactory();
            var unitShopPagePresenterFactory =
                new UnitShopPagePresenterFactory(unitShopService, unitShopMasterRepository, connectingView);
            var unitTypeInformationModalPresenterFactory =
                new UnitTypeInformationModalPresenterFactory(unitMasterRepository);
            var lockConfirmationModalPresenterFactory = new LockConfirmationModalPresenterFactory();

            // Transition Service
            var transitionService = new TransitionService(
                topPagePresenterFactory,
                homePagePresenterFactory,
                loadingPagePresenterFactory,
                unitShopPagePresenterFactory,
                settingsModalPresenterFactory,
                unitTypeInformationModalPresenterFactory,
                unitPortraitViewerModalPresenterFactory,
                lockConfirmationModalPresenterFactory
            );

            // Show Initial Page
            transitionService.ApplicationStarted();
        }
    }
}
