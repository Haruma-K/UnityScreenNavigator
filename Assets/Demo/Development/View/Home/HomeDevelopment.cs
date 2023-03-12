using Demo.Core.Scripts.View.Home;
using Demo.Development.View.Shared;
using UniRx;
using UnityEngine;

namespace Demo.Development.View.Home
{
    public sealed class HomeDevelopment : AppViewDevelopment<HomeView, HomeViewState>
    {
        protected override HomeViewState CreateState()
        {
            var state = new HomeViewState();
            state.SettingsButton.IsLocked.Value = false;
            state.SettingsButton.OnClicked.Subscribe(_ => Debug.Log("Settings Button Clicked")).AddTo(this);
            state.UnitShopButton.IsLocked.Value = false;
            state.UnitShopButton.OnClicked.Subscribe(_ => Debug.Log("Unit Shop Button Clicked")).AddTo(this);
            state.MainQuestButton.IsLocked.Value = true;
            state.MissionButton.IsLocked.Value = true;
            state.EventQuestButton.IsLocked.Value = true;
            state.OnBackButtonClicked.Subscribe(_ => Debug.Log("Back Button Clicked")).AddTo(this);
            return state;
        }
    }
}
