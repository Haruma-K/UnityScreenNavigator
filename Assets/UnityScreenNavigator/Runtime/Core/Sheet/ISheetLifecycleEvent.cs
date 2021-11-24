#if USN_USE_ASYNC_METHODS
using System.Threading.Tasks;
#else
using System.Collections;
#endif

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public interface ISheetLifecycleEvent
    {
#if USN_USE_ASYNC_METHODS
        Task Initialize();
#else
        IEnumerator Initialize();
#endif

#if USN_USE_ASYNC_METHODS
        Task WillEnter();
#else
        IEnumerator WillEnter();
#endif
        void DidEnter();

#if USN_USE_ASYNC_METHODS
        Task WillExit();
#else
        IEnumerator WillExit();
#endif

        void DidExit();

#if USN_USE_ASYNC_METHODS
        Task Cleanup();
#else
        IEnumerator Cleanup();
#endif
    }
}