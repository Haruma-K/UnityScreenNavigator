using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    internal sealed class PageLifecycleHandler
    {
        private readonly IEnumerable<IPageContainerCallbackReceiver> _callbackReceivers;
        private readonly RectTransform _containerTransform;

        public PageLifecycleHandler(
            RectTransform containerTransform,
            IEnumerable<IPageContainerCallbackReceiver> callbackReceivers
        )
        {
            _containerTransform = containerTransform;
            _callbackReceivers = callbackReceivers;
        }

        public IEnumerator AfterLoad(PagePushContext context)
        {
            yield return context.EnterPage.AfterLoad(_containerTransform);
        }

        public IEnumerator BeforePush(PagePushContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePush(context.EnterPage, context.ExitPage);

            var handles = new List<AsyncProcessHandle>();
            if (context.ExitPage != null)
                handles.Add(context.ExitPage.BeforeExit(true, context.EnterPage));

            handles.Add(context.EnterPage.BeforeEnter(true, context.ExitPage));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public IEnumerator Push(PagePushContext context, bool playAnimation)
        {
            var handles = new List<AsyncProcessHandle>();

            if (context.ExitPage != null)
                handles.Add(context.ExitPage.Exit(true, playAnimation, context.EnterPage));

            handles.Add(context.EnterPage.Enter(true, playAnimation, context.ExitPage));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public void AfterPush(PagePushContext context)
        {
            if (context.ExitPage != null)
                context.ExitPage.AfterExit(true, context.EnterPage);

            context.EnterPage.AfterEnter(true, context.ExitPage);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPush(context.EnterPage, context.ExitPage);
        }

        public IEnumerator AfterPushRoutine(PagePushContext context)
        {
            if (context.IsExitPageStacked || context.ExitPage == null)
                yield break;

            var handle = context.ExitPage.BeforeRelease();
            while (!handle.IsTerminated)
                yield return handle;
        }

        public IEnumerator BeforePop(PagePopContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePop(context.EnterPage, context.ExitPage);

            var handles = new List<AsyncProcessHandle>();
            foreach (var exitModal in context.ExitPages)
                handles.Add(exitModal.BeforeExit(false, context.EnterPage));

            if (context.EnterPage != null)
                handles.Add(context.EnterPage.BeforeEnter(false, context.ExitPage));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public IEnumerator Pop(PagePopContext context, bool playAnimation)
        {
            // When popping multiple pages at once, only play the animation for
            // the first page exiting (currently visible) and the page that is coming in.
            var handles = new List<AsyncProcessHandle>
            {
                context.ExitPage.Exit(false, playAnimation, context.EnterPage)
            };

            if (context.EnterPage != null)
                handles.Add(context.EnterPage.Enter(false, playAnimation, context.ExitPage));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public void AfterPop(PagePopContext context)
        {
            foreach (var exitModal in context.ExitPages)
                exitModal.AfterExit(false, context.EnterPage);
            if (context.EnterPage != null)
                context.EnterPage.AfterEnter(false, context.ExitPage);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPop(context.EnterPage, context.ExitPage);
        }

        public IEnumerator AfterPopRoutine(PagePopContext context)
        {
            var handles = context.ExitPages.Select(exitModal => exitModal.BeforeRelease());

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }
    }
}