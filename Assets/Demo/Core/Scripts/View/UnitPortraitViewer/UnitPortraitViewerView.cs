using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Core.Scripts.View.Parts.UnitPortrait;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.UnitPortraitViewer
{
    public sealed class UnitPortraitViewerView : AppView<UnitPortraitViewerViewState>
    {
        public UnitPortraitView portraitView;
        public Button closeButton;

        protected override async UniTask Initialize(UnitPortraitViewerViewState viewState)
        {
            var internalState = (IUnitPortraitViewerState)viewState;
            closeButton.SetOnClickDestination(internalState.InvokeOnCloseButtonClicked).AddTo(this);

            await portraitView.InitializeAsync(viewState.Portrait);
        }
    }
}
