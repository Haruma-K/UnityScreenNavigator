using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Home
{
    public sealed class HomeView : AppView<HomeViewState>
    {
        public HomeButtonView settingsButton;
        public HomeButtonView unitShopButton;
        public HomeButtonView mainQuestButton;
        public HomeButtonView missionButton;
        public HomeButtonView eventQuestButton;
        public Button backButton;

        protected override async UniTask Initialize(HomeViewState viewState)
        {
            var internalState = (IHomeState)viewState;
            var tasks = new List<UniTask>
            {
                settingsButton.InitializeAsync(viewState.SettingsButton),
                unitShopButton.InitializeAsync(viewState.UnitShopButton),
                mainQuestButton.InitializeAsync(viewState.MainQuestButton),
                missionButton.InitializeAsync(viewState.MissionButton),
                eventQuestButton.InitializeAsync(viewState.EventQuestButton)
            };
            await UniTask.WhenAll(tasks);

            backButton.SetOnClickDestination(internalState.InvokeBackButtonClicked).AddTo(this);
        }
    }
}
