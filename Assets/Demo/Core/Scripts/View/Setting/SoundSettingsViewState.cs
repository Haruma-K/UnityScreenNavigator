using Demo.Subsystem.PresentationFramework;
using R3;

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

        public ReactiveProperty<float> VoiceVolume => _voiceVolume;
        public ReactiveProperty<float> BgmVolume => _bgmVolume;
        public ReactiveProperty<float> SeVolume => _seVolume;
        public ReactiveProperty<bool> IsVoiceEnabled => _isVoiceEnabled;
        public ReactiveProperty<bool> IsBgmEnabled => _isBgmEnabled;
        public ReactiveProperty<bool> IsSeEnabled => _isSeEnabled;

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
