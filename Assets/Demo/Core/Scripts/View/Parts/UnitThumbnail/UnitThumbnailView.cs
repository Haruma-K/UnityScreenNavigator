using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Subsystem.PresentationFramework;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Parts.UnitThumbnail
{
    public sealed class UnitThumbnailView : AppView<UnitThumbnailViewState>
    {
        public Image image;
        public Button button;

        protected override async UniTask Initialize(UnitThumbnailViewState viewState)
        {
            var stateInternal = (IUnitThumbnailState)viewState;
            image.sprite = await Addressables.LoadAssetAsync<Sprite>(viewState.ImageResourceKey.Value);
            button.SetOnClickDestination(stateInternal.InvokeClicked).AddTo(this);
        }
    }
}
