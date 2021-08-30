using UnityScreenNavigator.Runtime.Core.Page;

namespace Demo.Scripts
{
    public class HomeLoadingPage : Page
    {
        public override void DidPushEnter()
        {
            // Transition to "Home".
            PageContainer.Of(transform).Push(ResourceKey.HomePagePrefab(), true);
        }
    }
}