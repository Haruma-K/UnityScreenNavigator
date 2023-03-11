using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Demo.Core.Scripts.APIGateway.Setting
{
    public sealed class SettingsAPIGateway
    {
        private const string VoiceVolumePrefsKey = "Demo_VoiceVolume";
        private const string BgmVolumePrefsKey = "Demo_BgmVolume";
        private const string SeVolumePrefsKey = "Demo_SeVolume";
        private const string IsVoiceMutedPrefsKey = "Demo_VoiceMuted";
        private const string IsBgmMutedPrefsKey = "Demo_BgmMuted";
        private const string IsSeMutedPrefsKey = "Demo_SeMuted";

        private static float VoiceVolume
        {
            get => PlayerPrefs.GetFloat(VoiceVolumePrefsKey, 1.0f);
            set => PlayerPrefs.SetFloat(VoiceVolumePrefsKey, value);
        }

        private static float BgmVolume
        {
            get => PlayerPrefs.GetFloat(BgmVolumePrefsKey, 1.0f);
            set => PlayerPrefs.SetFloat(BgmVolumePrefsKey, value);
        }

        private static float SeVolume
        {
            get => PlayerPrefs.GetFloat(SeVolumePrefsKey, 1.0f);
            set => PlayerPrefs.SetFloat(SeVolumePrefsKey, value);
        }

        private static bool IsVoiceMuted
        {
            get => PlayerPrefs.GetInt(IsVoiceMutedPrefsKey, 0) == 1;
            set => PlayerPrefs.SetInt(IsVoiceMutedPrefsKey, value ? 1 : 0);
        }

        private static bool IsBgmMuted
        {
            get => PlayerPrefs.GetInt(IsBgmMutedPrefsKey, 0) == 1;
            set => PlayerPrefs.SetInt(IsBgmMutedPrefsKey, value ? 1 : 0);
        }

        private static bool IsSeMuted
        {
            get => PlayerPrefs.GetInt(IsSeMutedPrefsKey, 0) == 1;
            set => PlayerPrefs.SetInt(IsSeMutedPrefsKey, value ? 1 : 0);
        }

        public UniTask<FetchSoundSettingsResponse> FetchSoundSettingsAsync()
        {
            var soundSettings = new FetchSoundSettingsResponse
            (
                VoiceVolume,
                BgmVolume,
                SeVolume,
                IsVoiceMuted,
                IsBgmMuted,
                IsSeMuted
            );
            return UniTask.FromResult(soundSettings);
        }

        public UniTask SaveSoundSettingsAsync(SaveSoundSettingsRequest request)
        {
            IsVoiceMuted = request.IsVoiceMuted;
            IsBgmMuted = request.IsBgmMuted;
            IsSeMuted = request.IsSeMuted;
            VoiceVolume = request.VoiceVolume;
            BgmVolume = request.BgmVolume;
            SeVolume = request.SeVolume;
            return UniTask.CompletedTask;
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

        #region Responses

        public readonly struct FetchSoundSettingsResponse
        {
            public FetchSoundSettingsResponse(float voiceVolume, float bgmVolume, float seVolume, bool isVoiceMuted,
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
