using System;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    public sealed class AnonymousPageContainerCallbackReceiver : IPageContainerCallbackReceiver
    {
        public AnonymousPageContainerCallbackReceiver(
            Action<(Page enterPage, Page exitPage)> onBeforePush = null,
            Action<(Page enterPage, Page exitPage)> onAfterPush = null,
            Action<(Page enterPage, Page exitPage)> onBeforePop = null,
            Action<(Page enterPage, Page exitPage)> onAfterPop = null)
        {
            OnBeforePush = onBeforePush;
            OnAfterPush = onAfterPush;
            OnBeforePop = onBeforePop;
            OnAfterPop = onAfterPop;
        }

        public void BeforePush(Page enterPage, Page exitPage)
        {
            OnBeforePush?.Invoke((enterPage, exitPage));
        }

        public void AfterPush(Page enterPage, Page exitPage)
        {
            OnAfterPush?.Invoke((enterPage, exitPage));
        }

        public void BeforePop(Page enterPage, Page exitPage)
        {
            OnBeforePop?.Invoke((enterPage, exitPage));
        }

        public void AfterPop(Page enterPage, Page exitPage)
        {
            OnAfterPop?.Invoke((enterPage, exitPage));
        }

        public event Action<(Page enterPage, Page exitPage)> OnAfterPop;
        public event Action<(Page enterPage, Page exitPage)> OnAfterPush;
        public event Action<(Page enterPage, Page exitPage)> OnBeforePop;
        public event Action<(Page enterPage, Page exitPage)> OnBeforePush;
    }
}