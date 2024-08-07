using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.PresentationFramework;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Confirmation
{
    public sealed class ConfirmationView : AppView<ConfirmationViewState>
    {
        public TMP_Text messageText;
        public Button closeButton;

        protected override UniTask Initialize(ConfirmationViewState viewState)
        {
            viewState.Message.Subscribe(x => messageText.text = x).AddTo(this);

            var internalState = (IConfirmationViewState)viewState;
            closeButton.SetOnClickDestination(internalState.InvokeCloseButtonClicked).AddTo(this);

            return UniTask.CompletedTask;
        }
    }
}