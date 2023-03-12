using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Top
{
    public sealed class TopView : AppView<TopViewState>
    {
        public Button button;

        protected override UniTask Initialize(TopViewState viewState)
        {
            var internalState = (ITopState)viewState;
            button.SetOnClickDestination(internalState.InvokeBackButtonClicked).AddTo(this);
            return UniTask.CompletedTask;
        }
    }
}
