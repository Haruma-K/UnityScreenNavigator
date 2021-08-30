using UnityEngine;
using UnityEngine.UI;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Demo.Scripts
{
    public class TopPage : Page
    {
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClick);
            }
        }

        private void OnClick()
        {
            PageContainer.Of(transform).Push(ResourceKey.HomeLoadingPagePrefab(), true, false);
        }
    }
}
