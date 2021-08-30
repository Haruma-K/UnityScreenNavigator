using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Page;

namespace DemoDevelopment
{
    public class PageDevelopmentManager : MonoBehaviour
    {
        [SerializeField] private string _resourceKey;
        [SerializeField] private PageContainer _pageContainer;

        private void Start()
        {
            _pageContainer.Push(_resourceKey, false);
        }
    }
}
