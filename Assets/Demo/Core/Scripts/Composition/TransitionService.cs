using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Foundation.Common;
using Demo.Core.Scripts.Presentation.Home;
using Demo.Core.Scripts.Presentation.Loading;
using Demo.Core.Scripts.Presentation.Setting;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.Presentation.Top;
using Demo.Core.Scripts.Presentation.UnitPortraitViewer;
using Demo.Core.Scripts.Presentation.UnitShop;
using Demo.Core.Scripts.Presentation.UnitTypeInformation;
using Demo.Core.Scripts.View.Home;
using Demo.Core.Scripts.View.Loading;
using Demo.Core.Scripts.View.Setting;
using Demo.Core.Scripts.View.Top;
using Demo.Core.Scripts.View.UnitPortraitViewer;
using Demo.Core.Scripts.View.UnitShop;
using Demo.Core.Scripts.View.UnitTypeInformation;
using Demo.Subsystem.PresentationFramework.UnityScreenNavigatorExtensions;
using UniRx;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Demo.Core.Scripts.Composition
{
    public sealed class TransitionService : ITransitionService
    {
        private readonly HomePagePresenterFactory _homePagePresenterFactory;
        private readonly LoadingPagePresenterFactory _loadingPagePresenterFactory;
        private readonly SettingsModalPresenterFactory _settingsModalPresenterFactory;
        private readonly TopPagePresenterFactory _topPagePresenterFactory;
        private readonly UnitPortraitViewerModalPresenterFactory _unitPortraitViewerModalPresenterFactory;
        private readonly UnitShopPagePresenterFactory _unitShopPagePresenterFactory;
        private readonly UnitTypeInformationModalPresenterFactory _unitTypeInformationModalPresenterFactory;

        public TransitionService(
            TopPagePresenterFactory topPagePresenterFactory,
            HomePagePresenterFactory homePagePresenterFactory,
            LoadingPagePresenterFactory loadingPagePresenterFactory,
            UnitShopPagePresenterFactory unitShopPagePresenterFactory,
            SettingsModalPresenterFactory settingsModalPresenterFactory,
            UnitTypeInformationModalPresenterFactory unitTypeInformationModalPresenterFactory,
            UnitPortraitViewerModalPresenterFactory unitPortraitViewerModalPresenterFactory
        )
        {
            _topPagePresenterFactory = topPagePresenterFactory;
            _homePagePresenterFactory = homePagePresenterFactory;
            _loadingPagePresenterFactory = loadingPagePresenterFactory;
            _unitShopPagePresenterFactory = unitShopPagePresenterFactory;
            _settingsModalPresenterFactory = settingsModalPresenterFactory;
            _unitTypeInformationModalPresenterFactory = unitTypeInformationModalPresenterFactory;
            _unitPortraitViewerModalPresenterFactory = unitPortraitViewerModalPresenterFactory;
        }

        private static PageContainer MainPageContainer => PageContainer.Find("MainPageContainer");
        private static ModalContainer MainModalContainer => ModalContainer.Find("MainModalContainer");

        public void ApplicationStarted()
        {
            MainPageContainer.Push<TopPage>(ResourceKey.Prefabs.TopPage, false,
                onLoad: page =>
                {
                    var presenter = _topPagePresenterFactory.Create(page, this);
                    OnPagePresenterCreated(presenter, page);
                });
        }

        public void TopPageClicked()
        {
            MainPageContainer.Push<LoadingPage>(ResourceKey.Prefabs.LoadingPage, true,
                onLoad: page =>
                {
                    var presenter = _loadingPagePresenterFactory.Create(page, this);
                    OnPagePresenterCreated(presenter, page);
                },
                stack: false);
        }

        public void HomeLoadingPageShown()
        {
            MainPageContainer.Push<HomePage>(ResourceKey.Prefabs.HomePage, true,
                onLoad: page =>
                {
                    async Task WillPushEnter()
                    {
                        // Preload the "Shop" page prefab.
                        await MainPageContainer.Preload(ResourceKey.Prefabs.UnitShopPage);
                        // Simulate loading time.
                        await UniTask.Delay(1000);
                    }

                    Task WillPopExit()
                    {
                        MainPageContainer.ReleasePreloaded(ResourceKey.Prefabs.UnitShopPage);
                        return Task.CompletedTask;
                    }

                    page.AddLifecycleEvent(onWillPushEnter: WillPushEnter, onWillPopExit: WillPopExit);
                    var presenter = _homePagePresenterFactory.Create(page, this);
                    OnPagePresenterCreated(presenter, page);
                });
        }

        public void HomePageUnitShopButtonClicked()
        {
            MainPageContainer.Push<UnitShopPage>(ResourceKey.Prefabs.UnitShopPage, true,
                onLoad: page =>
                {
                    var presenter = _unitShopPagePresenterFactory.Create(page, this);
                    OnPagePresenterCreated(presenter, page);
                });
        }

        public void HomePageSettingsButtonClicked()
        {
            MainModalContainer.Push<SettingsModal>(ResourceKey.Prefabs.SettingsModal, true,
                modal =>
                {
                    var presenter = _settingsModalPresenterFactory.Create(modal, this);
                    OnModalPresenterCreated(presenter, modal);
                });
        }

        public void UnitShopItemClicked(string unitTypeMasterId)
        {
            MainModalContainer.Push<UnitTypeInformationModal>(ResourceKey.Prefabs.UnitTypeInformationModal, true,
                modal =>
                {
                    var presenter = _unitTypeInformationModalPresenterFactory.Create(modal, this, unitTypeMasterId);
                    OnModalPresenterCreated(presenter, modal);
                });
        }

        public void UnitTypeInformationExpandButtonClicked(string unitTypeMasterId, int unitRank)
        {
            MainModalContainer.Push<UnitPortraitViewerModal>(ResourceKey.Prefabs.UnitPortraitViewerModal, true,
                modal =>
                {
                    var presenter =
                        _unitPortraitViewerModalPresenterFactory.Create(modal, this, unitTypeMasterId, unitRank);
                    OnModalPresenterCreated(presenter, modal);
                });
        }

        public void PopCommandExecuted()
        {
            if (MainModalContainer.IsInTransition || MainPageContainer.IsInTransition)
                throw new InvalidOperationException("Cannot pop page or modal while in transition.");

            if (MainModalContainer.Modals.Count >= 1)
                MainModalContainer.Pop(true);
            else if (MainPageContainer.Pages.Count >= 1)
                MainPageContainer.Pop(true);
            else
                throw new InvalidOperationException("Cannot pop page or modal because there is no page or modal.");
        }

        private IPagePresenter OnPagePresenterCreated(IPagePresenter presenter, Page page, bool shouldInitialize = true)
        {
            if (shouldInitialize)
            {
                ((IPresenter)presenter).Initialize();
                presenter.AddTo(page.gameObject);
            }

            return presenter;
        }

        private IModalPresenter OnModalPresenterCreated(IModalPresenter presenter, Modal modal,
            bool shouldInitialize = true)
        {
            if (shouldInitialize)
            {
                ((IPresenter)presenter).Initialize();
                presenter.AddTo(modal.gameObject);
            }

            return presenter;
        }
    }
}
