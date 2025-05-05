using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    internal sealed class SheetLifecycleHandler
    {
        private readonly RectTransform _containerTransform;
        private readonly IEnumerable<ISheetContainerCallbackReceiver> _callbackReceivers;

        public SheetLifecycleHandler(
            RectTransform containerTransform,
            IEnumerable<ISheetContainerCallbackReceiver> callbackReceivers
        )
        {
            _containerTransform = containerTransform;
            _callbackReceivers = callbackReceivers;
        }

        public IEnumerator AfterLoad(SheetRegisterContext context)
        {
            yield return context.Sheet.AfterLoad(_containerTransform);
        }

        public IEnumerator BeforeShow(SheetShowContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforeShow(context.EnterSheet, context.ExitSheet);
            
            var handles = new List<AsyncProcessHandle>();
            if (context.ExitSheet != null)
                handles.Add(context.ExitSheet.BeforeExit(context.EnterSheet));

            handles.Add(context.EnterSheet.BeforeEnter(context.ExitSheet));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return null;
        }

        public IEnumerator Show(SheetShowContext context, bool playAnimation)
        {
            var handles = new List<AsyncProcessHandle>();

            if (context.ExitSheet != null)
                handles.Add(context.ExitSheet.Exit(playAnimation, context.EnterSheet));

            handles.Add(context.EnterSheet.Enter(playAnimation, context.ExitSheet));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }
        
        public void AfterShow(SheetShowContext context)
        {
            if (context.ExitSheet != null)
                context.ExitSheet.AfterExit(context.EnterSheet);

            context.EnterSheet.AfterEnter(context.ExitSheet);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterShow(context.EnterSheet, context.ExitSheet);
        }

        public IEnumerator BeforeHide(SheetHideContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforeHide(context.ExitSheet);
            
            var handles = new List<AsyncProcessHandle>
            {
                context.ExitSheet.BeforeExit(null)
            };

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public IEnumerator Hide(SheetHideContext context, bool playAnimation)
        {
            var handle = context.ExitSheet.Exit(playAnimation, null);
            while (!handle.IsTerminated)
                yield return null;
        }

        public void AfterHide(SheetHideContext context)
        {
            context.ExitSheet.AfterExit(null);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterHide(context.ExitSheet);
        }
    }
} 