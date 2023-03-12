using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation.Binders;
using Demo.Core.Scripts.View.Parts.UnitThumbnail;
using Demo.Subsystem.PresentationFramework;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopItemView : AppView<UnitShopItemViewState>
    {
        public TMP_Text costText;
        public Image costIconImage;
        public UnitThumbnailView thumbnail;
        public GameObject lockedRoot;
        public GameObject unlockedRoot;
        public Button buyButton;
        public TMP_Text buyButtonText;

        protected override async UniTask Initialize(UnitShopItemViewState viewState)
        {
            lockedRoot.SetActiveSelfSource(viewState.IsLocked).AddTo(this);
            unlockedRoot.SetActiveSelfSource(viewState.IsLocked, true).AddTo(this);

            if (!viewState.IsLocked.Value)
            {
                costText.SetTextSource(viewState.Cost).AddTo(this);
                viewState.IsSoldOut.Subscribe(x =>
                {
                    buyButtonText.text = x ? "Sold Out" : "Buy";
                    buyButton.interactable = !x;
                }).AddTo(this);
                buyButton.onClick.AsObservable().Subscribe(_ => viewState.InvokeBuyButtonClicked()).AddTo(this);

                await UniTask.WhenAll
                (
                    // Load the cost icon
                    UniTask.Create(async () =>
                    {
                        var costIconResourceKey = viewState.CostIconImageResourceKey.Value;
                        costIconImage.sprite = await Addressables.LoadAssetAsync<Sprite>(costIconResourceKey);
                    }),
                    // Initialize the thumbnail
                    thumbnail.InitializeAsync(viewState.Thumbnail)
                );
            }
        }
    }
}
