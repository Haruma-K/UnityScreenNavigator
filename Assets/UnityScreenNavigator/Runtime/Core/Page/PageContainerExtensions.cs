using System;
using UnityScreenNavigator.Runtime.Core.Shared;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    public static class PageContainerExtensions
    {
        /// <summary>
        ///     Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(this PageContainer self,
            Action<(Page enterPage, Page exitPage)> onBeforePush = null,
            Action<(Page enterPage, Page exitPage)> onAfterPush = null,
            Action<(Page enterPage, Page exitPage)> onBeforePop = null,
            Action<(Page enterPage, Page exitPage)> onAfterPop = null)
        {
            var callbackReceiver =
                new AnonymousPageContainerCallbackReceiver(onBeforePush, onAfterPush, onBeforePop, onAfterPop);
            self.AddCallbackReceiver(callbackReceiver);
        }

        /// <summary>
        ///     Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="page"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(this PageContainer self, Page page,
            Action<Page> onBeforePush = null, Action<Page> onAfterPush = null,
            Action<Page> onBeforePop = null, Action<Page> onAfterPop = null)
        {
            var callbackReceiver = new AnonymousPageContainerCallbackReceiver();
            callbackReceiver.OnBeforePush += x =>
            {
                var (enterPage, exitPage) = x;
                if (enterPage.Equals(page))
                {
                    onBeforePush?.Invoke(exitPage);
                }
            };
            callbackReceiver.OnAfterPush += x =>
            {
                var (enterPage, exitPage) = x;
                if (enterPage.Equals(page))
                {
                    onAfterPush?.Invoke(exitPage);
                }
            };
            callbackReceiver.OnBeforePop += x =>
            {
                var (enterPage, exitPage) = x;
                if (exitPage.Equals(page))
                {
                    onBeforePop?.Invoke(enterPage);
                }
            };
            callbackReceiver.OnAfterPop += x =>
            {
                var (enterPage, exitPage) = x;
                if (exitPage.Equals(page))
                {
                    onAfterPop?.Invoke(enterPage);
                }
            };

            var gameObj = self.gameObject;
            if (!gameObj.TryGetComponent<MonoBehaviourDestroyedEventDispatcher>(out var destroyedEventDispatcher))
            {
                destroyedEventDispatcher = gameObj.AddComponent<MonoBehaviourDestroyedEventDispatcher>();
            }

            destroyedEventDispatcher.OnDispatch += () => self.RemoveCallbackReceiver(callbackReceiver);

            self.AddCallbackReceiver(callbackReceiver);
        }
    }
}