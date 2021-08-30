using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Page;

namespace Demo.Scripts
{
    public class RootPageManager : MonoBehaviour
    {
        [SerializeField] private PageContainer _pageContainer;

        private void Start()
        {
            _pageContainer.Push(ResourceKey.TopPagePrefab(), false, loadAsync: false);
        }
    }
}