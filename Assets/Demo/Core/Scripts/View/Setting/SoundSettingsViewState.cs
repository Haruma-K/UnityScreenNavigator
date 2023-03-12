using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.Setting
{
    public sealed class SoundSettingsViewState : AppViewState, ISoundSettingsState
    {
        private readonly ReactiveProperty<float> _bgmVolume = new ReactiveProperty<float>();
        private readonly ReactiveProperty<bool> _isBgmEnabled = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _isSeEnabled = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _isVoiceEnabled = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<float> _seVolume = new ReactiveProperty<float>();
        private readonly ReactiveProperty<float> _voiceVolume = new ReactiveProperty<float>();

        public IReactiveProperty<float> VoiceVolume => _voiceVolume;
        public IReactiveProperty<float> BgmVolume => _bgmVolume;
        public IReactiveProperty<float> SeVolume => _seVolume;
        public IReactiveProperty<bool> IsVoiceEnabled => _isVoiceEnabled;
        public IReactiveProperty<bool> IsBgmEnabled => _isBgmEnabled;
        public IReactiveProperty<bool> IsSeEnabled => _isSeEnabled;

        protected override void DisposeInternal()
        {
            _voiceVolume.Dispose();
            _bgmVolume.Dispose();
            _seVolume.Dispose();
            _isVoiceEnabled.Dispose();
            _isBgmEnabled.Dispose();
            _isSeEnabled.Dispose();
        }
    }

    internal interface ISoundSettingsState
    {
    }
}
