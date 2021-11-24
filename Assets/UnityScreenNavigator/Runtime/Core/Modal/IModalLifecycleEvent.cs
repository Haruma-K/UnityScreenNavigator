using System.Collections;
#if USN_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public interface IModalLifecycleEvent
    {
#if USN_USE_ASYNC_METHODS
        Task Initialize();
#else
        IEnumerator Initialize();
#endif

#if USN_USE_ASYNC_METHODS
        Task WillPushEnter();
#else
        IEnumerator WillPushEnter();
#endif

        void DidPushEnter();

#if USN_USE_ASYNC_METHODS
        Task WillPushExit();
#else
        IEnumerator WillPushExit();
#endif

        void DidPushExit();

#if USN_USE_ASYNC_METHODS
        Task WillPopEnter();
#else
        IEnumerator WillPopEnter();
#endif

        void DidPopEnter();

#if USN_USE_ASYNC_METHODS
        Task WillPopExit();
#else
        IEnumerator WillPopExit();
#endif

        void DidPopExit();

#if USN_USE_ASYNC_METHODS
        Task Cleanup();
#else
        IEnumerator Cleanup();
#endif
    }
}