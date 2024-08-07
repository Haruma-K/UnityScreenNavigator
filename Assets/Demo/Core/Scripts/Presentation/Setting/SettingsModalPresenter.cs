using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.UseCase.Setting;
using Demo.Core.Scripts.View.Setting;
using Demo.Subsystem.Misc;
using UniRx;

namespace Demo.Core.Scripts.Presentation.Setting
{
    public sealed class SettingsModalPresenter : ModalPresenterBase<SettingsModal, SettingsView, SettingsViewState>
    {
        private readonly SettingsUseCase _useCase;
        private bool _dirty;

        public SettingsModalPresenter(SettingsModal view, ITransitionService transitionService, SettingsUseCase useCase)
            : base(view, transitionService)
        {
            _useCase = useCase;
        }

        protected override async Task ViewDidLoad(SettingsModal view, SettingsViewState viewState)
        {
            // Update models.
            await _useCase.FetchSoundSettingsAsync();
            var model = _useCase.Model;

            // Set view state with initial values.
            SetVoiceSettingsViewState(viewState, model.Sounds.Voice.Volume, model.Sounds.Voice.Muted);
            SetBgmSettingsViewState(viewState, model.Sounds.Bgm.Volume, model.Sounds.Bgm.Muted);
            SetSeSettingsViewState(viewState, model.Sounds.Se.Volume, model.Sounds.Se.Muted);

            // Observe changes of models.
            model.Sounds.Voice
                .ValueChanged
                .Subscribe(x => SetVoiceSettingsViewState(viewState, x.Volume, x.Muted))
                .AddTo(this);
            model.Sounds.Bgm
                .ValueChanged
                .Subscribe(x => SetBgmSettingsViewState(viewState, x.Volume, x.Muted))
                .AddTo(this);
            model.Sounds.Se
                .ValueChanged
                .Subscribe(x => SetSeSettingsViewState(viewState, x.Volume, x.Muted))
                .AddTo(this);

            // Observe changes of view state.
            viewState.SoundSettings.IsVoiceEnabled.Subscribe(_ => _dirty = true).AddTo(this);
            viewState.SoundSettings.IsBgmEnabled.Subscribe(_ => _dirty = true).AddTo(this);
            viewState.SoundSettings.IsSeEnabled.Subscribe(_ => _dirty = true).AddTo(this);
            viewState.SoundSettings.VoiceVolume.Subscribe(_ => _dirty = true).AddTo(this);
            viewState.SoundSettings.SeVolume.Subscribe(_ => _dirty = true).AddTo(this);
            viewState.SoundSettings.BgmVolume.Subscribe(_ => _dirty = true).AddTo(this);
            viewState.CloseButtonClicked
                .Subscribe(_ => TransitionService.PopCommandExecuted())
                .AddTo(this);
            viewState.LockedButtonClicked
                .Subscribe(_ => TransitionService.SettingsModalLockedButtonClicked())
                .AddTo(this);
        }

        private void SetVoiceSettingsViewState(SettingsViewState viewState, float volume, bool isMuted)
        {
            viewState.SoundSettings.VoiceVolume.Value = volume;
            viewState.SoundSettings.IsVoiceEnabled.Value = !isMuted;
        }

        private void SetBgmSettingsViewState(SettingsViewState viewState, float volume, bool isMuted)
        {
            viewState.SoundSettings.BgmVolume.Value = volume;
            viewState.SoundSettings.IsBgmEnabled.Value = !isMuted;
        }

        private void SetSeSettingsViewState(SettingsViewState viewState, float volume, bool isMuted)
        {
            viewState.SoundSettings.SeVolume.Value = volume;
            viewState.SoundSettings.IsSeEnabled.Value = !isMuted;
        }

        protected override async Task ViewWillPopExit(SettingsModal view, SettingsViewState viewState)
        {
            await ViewWillExit(view, viewState);
        }

        protected override async Task ViewWillPushExit(SettingsModal view, SettingsViewState viewState)
        {
            await ViewWillExit(view, viewState);
        }

        private async UniTask ViewWillExit(SettingsModal view, SettingsViewState viewState)
        {
            if (!_dirty)
                return;

            // Save sound settings
            await _useCase.SaveSoundSettingsAsync
            (
                new SettingsUseCase.SaveSoundSettingsRequest(
                    viewState.SoundSettings.VoiceVolume.Value,
                    viewState.SoundSettings.BgmVolume.Value,
                    viewState.SoundSettings.SeVolume.Value,
                    !viewState.SoundSettings.IsVoiceEnabled.Value,
                    !viewState.SoundSettings.IsBgmEnabled.Value,
                    !viewState.SoundSettings.IsSeEnabled.Value
                )
            );
        }
    }
}
