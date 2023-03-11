using Demo.Core.Scripts.Domain.UnitShop.MasterRepository;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.UseCase.UnitShop;
using Demo.Core.Scripts.View.Overlay;
using Demo.Core.Scripts.View.UnitShop;

namespace Demo.Core.Scripts.Presentation.UnitShop
{
    public sealed class UnitShopPagePresenterFactory
    {
        private readonly IUnitShopMasterRepository _unitShopMasterRepository;
        private readonly UnitShopUseCase _unitShopUseCase;
        private readonly ConnectingView _connectingView;

        public UnitShopPagePresenterFactory(UnitShopUseCase unitShopUseCase,
            IUnitShopMasterRepository unitShopMasterRepository, ConnectingView connectingView)
        {
            _unitShopUseCase = unitShopUseCase;
            _unitShopMasterRepository = unitShopMasterRepository;
            _connectingView = connectingView;
        }

        public UnitShopPagePresenter Create(UnitShopPage view, ITransitionService transitionService)
        {
            return new UnitShopPagePresenter(view, transitionService, _unitShopUseCase, _unitShopMasterRepository
                , _connectingView);
        }
    }
}
