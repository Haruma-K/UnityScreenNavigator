using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Demo.Scripts
{
    public class HomePage : Page
    {
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _shopButton;

        public override IEnumerator Initialize()
        {
            _settingButton.onClick.AddListener(OnSettingButtonClicked);
            _shopButton.onClick.AddListener(OnShopButtonClicked);

            // Preload the "Shop" page prefab.
            yield return PageContainer.Of(transform).Preload(ResourceKey.ShopPagePrefab());
            // Simulate loading time.
            yield return new WaitForSeconds(1.0f);
        }

        public override IEnumerator Cleanup()
        {
            _settingButton.onClick.RemoveListener(OnSettingButtonClicked);
            _shopButton.onClick.RemoveListener(OnShopButtonClicked);
            PageContainer.Of(transform).ReleasePreloaded(ResourceKey.ShopPagePrefab());
            yield break;
        }

        private void OnSettingButtonClicked()
        {
            ModalContainer.Find(ContainerKey.MainModalContainer).Push(ResourceKey.SettingsModalPrefab(), true);
        }

        private void OnShopButtonClicked()
        {
            PageContainer.Of(transform).Push(ResourceKey.ShopPagePrefab(), true);
        }
    }
}