using System.Collections.Generic;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    internal sealed class PageLifecycleHandler
    {
        private readonly IEnumerable<IPageContainerCallbackReceiver> _callbackReceivers;

        public PageLifecycleHandler(IEnumerable<IPageContainerCallbackReceiver> callbackReceivers)
        {
            _callbackReceivers = callbackReceivers;
        }

        public IEnumerable<AsyncProcessHandle> BeforePush(PagePushContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePush(context.EnterPage, context.ExitPage);

            var handles = new List<AsyncProcessHandle>();
            if (context.ExitPage != null)
                handles.Add(context.ExitPage.BeforeExit(true, context.EnterPage));

            handles.Add(context.EnterPage.BeforeEnter(true, context.ExitPage));

            return handles;
        }

        public void AfterPush(PagePushContext context)
        {
            if (context.ExitPage != null)
                context.ExitPage.AfterExit(true, context.EnterPage);

            context.EnterPage.AfterEnter(true, context.ExitPage);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPush(context.EnterPage, context.ExitPage);
        }

        public IEnumerable<AsyncProcessHandle> BeforePop(PagePopContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePop(context.EnterPage, context.ExitPage);
            
            var handles = new List<AsyncProcessHandle>
            {
                context.ExitPage.BeforeExit(false, context.EnterPage)
            };

            if (context.EnterPage != null)
                handles.Add(context.EnterPage.BeforeEnter(false, context.ExitPage));

            return handles;
        }

        public void AfterPop(PagePopContext context)
        {
            context.ExitPage.AfterExit(false, context.EnterPage);
            if (context.EnterPage != null)
                context.EnterPage.AfterEnter(false, context.ExitPage);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPop(context.EnterPage, context.ExitPage);
        }
    }
} 