using Cysharp.Threading.Tasks;
using Demo.Subsystem.PresentationFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Parts.UnitPortrait
{
    public sealed class UnitPortraitView : AppView<UnitPortraitViewState>
    {
        public Image image;

        protected override async UniTask Initialize(UnitPortraitViewState viewState)
        {
            image.sprite = await Addressables.LoadAssetAsync<Sprite>(viewState.ImageResourceKey.Value);
        }
    }
}
