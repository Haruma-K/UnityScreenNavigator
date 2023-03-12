using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Domain.UnitShop.MasterRepository;
using Demo.Core.Scripts.Domain.UnitShop.Model;
using Demo.Core.Scripts.Foundation.Common;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.UseCase.UnitShop;
using Demo.Core.Scripts.View.Overlay;
using Demo.Core.Scripts.View.UnitShop;
using Demo.Subsystem.Misc;
using UniRx;

namespace Demo.Core.Scripts.Presentation.UnitShop
{
    public sealed class UnitShopPagePresenter : PagePresenterBase<UnitShopPage, UnitShopView, UnitShopViewState>
    {
        private readonly IUnitShopMasterRepository _unitShopMasterRepository;
        private readonly UnitShopUseCase _unitShopUseCase;
        private readonly ConnectingView _connectingView;

        public UnitShopPagePresenter(UnitShopPage view, ITransitionService transitionService,
            UnitShopUseCase unitShopUseCase, IUnitShopMasterRepository unitShopMasterRepository,
            ConnectingView connectingView)
            : base(view, transitionService)
        {
            _unitShopUseCase = unitShopUseCase;
            _unitShopMasterRepository = unitShopMasterRepository;
            _connectingView = connectingView;
        }

        protected override async Task ViewDidLoad(UnitShopPage view, UnitShopViewState viewState)
        {
            IUnitShopItemMasterTable masterTable = null;
            await UniTask.WhenAll(
                UniTask.Create(async () => masterTable = await _unitShopMasterRepository.FetchItemTableAsync()),
                _unitShopUseCase.FetchItemSetAsync()
            );
            var model = _unitShopUseCase.Model;

            SetupShopItemSetView(viewState.RegularItems, model.RegularItems, masterTable);
            SetupShopItemSetView(viewState.SpecialItems, model.SpecialItems, masterTable);
            SetupShopItemSetView(viewState.SaleItems, model.SaleItems, masterTable);

            viewState.OnBackButtonClicked
                .Subscribe(_ => TransitionService.PopCommandExecuted())
                .AddTo(this);
        }

        private void SetupShopItemSetView(UnitShopItemSetViewState viewState,
            IReadOnlyList<UnitShopItem> shopItemModels, IUnitShopItemMasterTable masterTable)
        {
            var shopItemViewStates = new List<UnitShopItemViewState>
            {
                viewState.Item1,
                viewState.Item2,
                viewState.Item3,
                viewState.Item4,
                viewState.Item5,
                viewState.Item6,
                viewState.Item7,
                viewState.Item8
            };
            for (var i = 0; i < shopItemViewStates.Count; i++)
            {
                var shopItemViewState = shopItemViewStates[i];
                if (shopItemModels.Count <= i)
                {
                    SetupLockedUnitShopItemView(shopItemViewState);
                }
                else
                {
                    var shopItemModel = shopItemModels[i];
                    var shopItemMaster = masterTable.FindById(shopItemModel.MasterId);
                    SetupUnlockedUnitShopItemView(shopItemViewState, shopItemModel, shopItemMaster);
                }
            }
        }

        private void SetupUnlockedUnitShopItemView(UnitShopItemViewState viewState, UnitShopItem model,
            UnitShopItemMaster master)
        {
            var unitTypeMasterId = master.UnitTypeMasterId;
            viewState.IsLocked.Value = false;
            viewState.Thumbnail.ImageResourceKey.Value = ResourceKey.Textures.GetUnitThumbnail(unitTypeMasterId, 1);
            viewState.Thumbnail.OnClicked
                .Subscribe(_ => TransitionService.UnitShopItemClicked(unitTypeMasterId))
                .AddTo(this);
            viewState.Cost.Value = master.Cost;
            viewState.CostIconImageResourceKey.Value = ResourceKey.Textures.CoinIcon;
            viewState.IsSoldOut.Value = model.IsSoldOut;

            viewState.OnBuyButtonClicked
                .Subscribe(_ => BuyAsync().Forget())
                .AddTo(this);

            model.ValueChanged
                .Subscribe(_ => viewState.IsSoldOut.Value = model.IsSoldOut)
                .AddTo(this);

            async UniTask BuyAsync()
            {
                await _connectingView.ShowAsync();
                var request = new UnitShopUseCase.PurchaseItemRequest(model.Id);
                await _unitShopUseCase.PurchaseItemAsync(request);
                viewState.IsSoldOut.Value = true;
                await _connectingView.HideAsync();
            }
        }

        private void SetupLockedUnitShopItemView(UnitShopItemViewState viewState)
        {
            viewState.IsLocked.Value = true;
            viewState.CostIconImageResourceKey.Value = ResourceKey.Textures.CoinIcon;
            viewState.IsSoldOut.Value = false;
        }
    }
}
