using Demo.Core.Scripts.View.Setting;
using Demo.Development.View.Shared;
using UniRx;
using UnityEngine;

namespace Demo.Development.View.Settings
{
    public sealed class SettingsDevelopment : AppViewDevelopment<SettingsView, SettingsViewState>
    {
        protected override SettingsViewState CreateState()
        {
            var state = new SettingsViewState();
            state.SoundSettings.VoiceVolume.Value = 0.5f;
            state.SoundSettings.BgmVolume.Value = 1.0f;
            state.SoundSettings.SeVolume.Value = 0.0f;
            state.SoundSettings.IsVoiceEnabled.Value = true;
            state.SoundSettings.IsBgmEnabled.Value = true;
            state.SoundSettings.IsSeEnabled.Value = false;
            state.SoundSettings
                .VoiceVolume
                .Subscribe(volume => Debug.Log($"Voice volume: {volume}"))
                .AddTo(this);
            state.SoundSettings
                .BgmVolume
                .Subscribe(volume => Debug.Log($"BGM volume: {volume}"))
                .AddTo(this);
            state.SoundSettings
                .SeVolume
                .Subscribe(volume => Debug.Log($"SE volume: {volume}"))
                .AddTo(this);

            state.CloseButtonClicked
                .Subscribe(_ => Debug.Log("Close button clicked"))
                .AddTo(this);
            return state;
        }
    }
}
