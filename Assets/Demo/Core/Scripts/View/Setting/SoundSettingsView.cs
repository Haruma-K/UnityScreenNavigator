using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Setting
{
    public sealed class SoundSettingsView : AppView<SoundSettingsViewState>
    {
        public Slider voiceSlider;
        public Slider bgmSlider;
        public Slider seSlider;
        public Toggle voiceToggle;
        public Toggle bgmToggle;
        public Toggle seToggle;

        protected override UniTask Initialize(SoundSettingsViewState viewState)
        {
            viewState.VoiceVolume.Subscribe(x => voiceSlider.value = x).AddTo(this);
            viewState.BgmVolume.Subscribe(x => bgmSlider.value = x).AddTo(this);
            viewState.SeVolume.Subscribe(x => seSlider.value = x).AddTo(this);
            viewState.IsVoiceEnabled.Subscribe(x => voiceToggle.isOn = x).AddTo(this);
            viewState.IsBgmEnabled.Subscribe(x => bgmToggle.isOn = x).AddTo(this);
            viewState.IsSeEnabled.Subscribe(x => seToggle.isOn = x).AddTo(this);

            viewState.IsVoiceEnabled.Subscribe(x => voiceSlider.interactable = x).AddTo(this);
            viewState.IsBgmEnabled.Subscribe(x => bgmSlider.interactable = x).AddTo(this);
            viewState.IsSeEnabled.Subscribe(x => seSlider.interactable = x).AddTo(this);

            voiceSlider.SetOnValueChangedDestination(x => viewState.VoiceVolume.Value = x).AddTo(this);
            bgmSlider.SetOnValueChangedDestination(x => viewState.BgmVolume.Value = x).AddTo(this);
            seSlider.SetOnValueChangedDestination(x => viewState.SeVolume.Value = x).AddTo(this);
            voiceToggle.SetOnValueChangedDestination(x => viewState.IsVoiceEnabled.Value = x).AddTo(this);
            bgmToggle.SetOnValueChangedDestination(x => viewState.IsBgmEnabled.Value = x).AddTo(this);
            seToggle.SetOnValueChangedDestination(x => viewState.IsSeEnabled.Value = x).AddTo(this);

            return UniTask.CompletedTask;
        }
    }
}
