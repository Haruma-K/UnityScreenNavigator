using Demo.Core.Scripts.Domain.Unit.MasterRepository;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.UnitTypeInformation;

namespace Demo.Core.Scripts.Presentation.UnitTypeInformation
{
    public sealed class UnitTypeInformationModalPresenterFactory
    {
        private readonly IUnitMasterRepository _unitMasterRepository;

        public UnitTypeInformationModalPresenterFactory(IUnitMasterRepository unitMasterRepository)
        {
            _unitMasterRepository = unitMasterRepository;
        }
        
        public UnitTypeInformationModalPresenter Create(UnitTypeInformationModal view,
            ITransitionService transitionService, string unitTypeMasterId)
        {
            return new UnitTypeInformationModalPresenter(view, transitionService, _unitMasterRepository,
                unitTypeMasterId);
        }
    }
}
