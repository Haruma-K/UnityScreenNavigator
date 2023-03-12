using System.Threading.Tasks;
using Demo.Core.Scripts.Domain.Unit.MasterRepository;
using Demo.Core.Scripts.Foundation.Common;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.UnitTypeInformation;
using Demo.Subsystem.Misc;
using UniRx;

namespace Demo.Core.Scripts.Presentation.UnitTypeInformation
{
    public sealed class UnitTypeInformationModalPresenter : ModalPresenterBase<UnitTypeInformationModal,
        UnitTypeInformationView, UnitTypeInformationViewState>
    {
        private readonly IUnitMasterRepository _unitMasterRepository;
        private readonly string _unitTypeMasterId;

        public UnitTypeInformationModalPresenter(UnitTypeInformationModal view, ITransitionService transitionService,
            IUnitMasterRepository unitMasterRepository, string unitTypeMasterId) : base(view, transitionService)
        {
            _unitTypeMasterId = unitTypeMasterId;
            _unitMasterRepository = unitMasterRepository;
        }

        protected override async Task ViewDidLoad(UnitTypeInformationModal view, UnitTypeInformationViewState viewState)
        {
            var unitTypeTable = await _unitMasterRepository.FetchUnitTypeTableAsync();
            var unitTypeMaster = unitTypeTable.FindById(_unitTypeMasterId);

            viewState.Title.Value = unitTypeMaster.Name;
            viewState.Rank1Thumbnail.ImageResourceKey.Value =
                ResourceKey.Textures.GetUnitThumbnail(_unitTypeMasterId, 1);
            viewState.Rank2Thumbnail.ImageResourceKey.Value =
                ResourceKey.Textures.GetUnitThumbnail(_unitTypeMasterId, 2);
            viewState.Rank3Thumbnail.ImageResourceKey.Value =
                ResourceKey.Textures.GetUnitThumbnail(_unitTypeMasterId, 3);
            viewState.Rank1Description.Value = unitTypeMaster.Rank1Description;
            viewState.Rank2Description.Value = unitTypeMaster.Rank2Description;
            viewState.Rank3Description.Value = unitTypeMaster.Rank3Description;
            viewState.Rank1Portrait.ImageResourceKey.Value = ResourceKey.Textures.GetUnitPortrait(_unitTypeMasterId, 1);
            viewState.Rank2Portrait.ImageResourceKey.Value = ResourceKey.Textures.GetUnitPortrait(_unitTypeMasterId, 2);
            viewState.Rank3Portrait.ImageResourceKey.Value = ResourceKey.Textures.GetUnitPortrait(_unitTypeMasterId, 3);

            viewState.OnCloseButtonClicked
                .Subscribe(_ => TransitionService.PopCommandExecuted())
                .AddTo(this);
            viewState.OnExpandButtonClicked
                .Subscribe(_ =>
                {
                    var unitRank = viewState.TabIndex.Value + 1;
                    TransitionService.UnitTypeInformationExpandButtonClicked(_unitTypeMasterId, unitRank);
                })
                .AddTo(this);
        }
    }
}
