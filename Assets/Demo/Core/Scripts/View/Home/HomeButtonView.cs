using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Home
{
    public sealed class HomeButtonView : AppView<HomeButtonViewState>
    {
        public Button button;
        public GameObject lockedRoot;
        public GameObject unlockedRoot;

        protected override UniTask Initialize(HomeButtonViewState viewState)
        {
            var internalState = (IHomeButtonState)viewState;
            lockedRoot.SetActiveSelfSource(viewState.IsLocked).AddTo(this);
            unlockedRoot.SetActiveSelfSource(viewState.IsLocked, true).AddTo(this);
            button.SetOnClickDestination(internalState.InvokeClicked).AddTo(this);
            return UniTask.CompletedTask;
        }
    }
}
