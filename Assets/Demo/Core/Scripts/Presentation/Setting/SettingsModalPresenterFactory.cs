using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.UseCase.Setting;
using Demo.Core.Scripts.View.Setting;

namespace Demo.Core.Scripts.Presentation.Setting
{
    public sealed class SettingsModalPresenterFactory
    {
        private readonly SettingsUseCase _settingsUseCase;

        public SettingsModalPresenterFactory(SettingsUseCase settingsUseCase)
        {
            _settingsUseCase = settingsUseCase;
        }

        public SettingsModalPresenter Create(SettingsModal view, ITransitionService transitionService)
        {
            return new SettingsModalPresenter(view, transitionService, _settingsUseCase);
        }
    }
}
