using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Setting
{
    public sealed class SettingsView : AppView<SettingsViewState>
    {
        public Button closeButton;
        public Button[] lockedButtons;
        public SoundSettingsView soundSettingsView;

        protected override async UniTask Initialize(SettingsViewState viewState)
        {
            var internalState = (ISettingsState)viewState;
            closeButton.SetOnClickDestination(internalState.InvokeCloseButtonClicked).AddTo(this);
            foreach (var lockedButton in lockedButtons)
                lockedButton.SetOnClickDestination(internalState.InvokeLockedButtonClicked).AddTo(this);
            await soundSettingsView.InitializeAsync(viewState.SoundSettings);
        }
    }
}
