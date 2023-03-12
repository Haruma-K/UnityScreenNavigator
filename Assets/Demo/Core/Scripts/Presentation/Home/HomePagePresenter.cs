using System;
using System.Threading.Tasks;
using Demo.Core.Scripts.Domain.FeatureFlag.MasterRepository;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Home;
using Demo.Subsystem.Misc;
using UniRx;

namespace Demo.Core.Scripts.Presentation.Home
{
    public sealed class HomePagePresenter : PagePresenterBase<HomePage, HomeView, HomeViewState>
    {
        private readonly IFeatureFlagMasterRepository _featureFlagMasterRepository;

        public HomePagePresenter(HomePage view, ITransitionService transitionService,
            IFeatureFlagMasterRepository featureFlagMasterRepository) : base(view, transitionService)
        {
            _featureFlagMasterRepository = featureFlagMasterRepository;
        }

        protected override async Task ViewDidLoad(HomePage view, HomeViewState viewState)
        {
            var featureFlagMasterTable = await _featureFlagMasterRepository.FetchTableAsync();

            var isUnitShopEnabled = featureFlagMasterTable.FindById("unit_shop")?.Enabled ?? false;
            var isSettingsEnabled = featureFlagMasterTable.FindById("settings")?.Enabled ?? false;
            var isMainQuestEnabled = featureFlagMasterTable.FindById("main_quest")?.Enabled ?? false;
            var isEventQuestEnabled = featureFlagMasterTable.FindById("event_quest")?.Enabled ?? false;
            var isMissionEnabled = featureFlagMasterTable.FindById("mission")?.Enabled ?? false;

            viewState.UnitShopButton.IsLocked.Value = !isUnitShopEnabled;
            viewState.SettingsButton.IsLocked.Value = !isSettingsEnabled;
            viewState.MainQuestButton.IsLocked.Value = !isMainQuestEnabled;
            viewState.EventQuestButton.IsLocked.Value = !isEventQuestEnabled;
            viewState.MissionButton.IsLocked.Value = !isMissionEnabled;

            viewState.UnitShopButton
                .OnClicked
                .Subscribe(_ => TransitionService.HomePageUnitShopButtonClicked())
                .AddTo(this);
            viewState.SettingsButton
                .OnClicked
                .Subscribe(_ => TransitionService.HomePageSettingsButtonClicked())
                .AddTo(this);
            viewState.MainQuestButton
                .OnClicked
                .Subscribe(_ => throw new NotImplementedException())
                .AddTo(this);
            viewState.EventQuestButton
                .OnClicked
                .Subscribe(_ => throw new NotImplementedException())
                .AddTo(this);
            viewState.MissionButton
                .OnClicked
                .Subscribe(_ => throw new NotImplementedException())
                .AddTo(this);
            viewState.OnBackButtonClicked
                .Subscribe(_ => TransitionService.PopCommandExecuted())
                .AddTo(this);
        }
    }
}
