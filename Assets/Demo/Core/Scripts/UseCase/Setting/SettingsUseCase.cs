using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.APIGateway.Setting;
using Demo.Core.Scripts.Domain.Setting.Model;

namespace Demo.Core.Scripts.UseCase.Setting
{
    public sealed class SettingsUseCase
    {
        private readonly SettingsAPIGateway _apiGateway;

        public SettingsUseCase(Settings model, SettingsAPIGateway apiGateway)
        {
            Model = model;
            _apiGateway = apiGateway;
        }

        public Settings Model { get; }

        public async UniTask FetchSoundSettingsAsync()
        {
            var response = await _apiGateway.FetchSoundSettingsAsync();
            var sounds = Model.Sounds;
            sounds.Voice.SetValues(response.VoiceVolume, response.IsVoiceMuted);
            sounds.Bgm.SetValues(response.BgmVolume, response.IsBgmMuted);
            sounds.Se.SetValues(response.SeVolume, response.IsSeMuted);
        }

        public async UniTask SaveSoundSettingsAsync(SaveSoundSettingsRequest request)
        {
            var sounds = Model.Sounds;
            sounds.Voice.SetValues(request.VoiceVolume, request.IsVoiceMuted);
            sounds.Bgm.SetValues(request.BgmVolume, request.IsBgmMuted);
            sounds.Se.SetValues(request.SeVolume, request.IsSeMuted);
            var apiRequest = new SettingsAPIGateway.SaveSoundSettingsRequest(request.VoiceVolume, request.BgmVolume,
                request.SeVolume, request.IsVoiceMuted, request.IsBgmMuted, request.IsSeMuted);
            await _apiGateway.SaveSoundSettingsAsync(apiRequest);
        }

        #region Requests

        public readonly struct SaveSoundSettingsRequest
        {
            public SaveSoundSettingsRequest(float voiceVolume, float bgmVolume, float seVolume, bool isVoiceMuted,
                bool isBgmMuted, bool isSeMuted)
            {
                VoiceVolume = voiceVolume;
                BgmVolume = bgmVolume;
                SeVolume = seVolume;
                IsVoiceMuted = isVoiceMuted;
                IsBgmMuted = isBgmMuted;
                IsSeMuted = isSeMuted;
            }

            public float VoiceVolume { get; }
            public float BgmVolume { get; }
            public float SeVolume { get; }
            public bool IsVoiceMuted { get; }
            public bool IsBgmMuted { get; }
            public bool IsSeMuted { get; }
        }

        #endregion
    }
}
