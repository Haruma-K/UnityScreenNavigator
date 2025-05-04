using System.Collections.Generic;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    internal sealed class SheetLifecycleHandler
    {
        private readonly IEnumerable<ISheetContainerCallbackReceiver> _callbackReceivers;

        public SheetLifecycleHandler(IEnumerable<ISheetContainerCallbackReceiver> callbackReceivers)
        {
            _callbackReceivers = callbackReceivers;
        }

        public IEnumerable<AsyncProcessHandle> BeforeShow(SheetShowContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforeShow(context.EnterSheet, context.ExitSheet);
            
            var handles = new List<AsyncProcessHandle>();
            if (context.ExitSheet != null)
                handles.Add(context.ExitSheet.BeforeExit(context.EnterSheet));

            handles.Add(context.EnterSheet.BeforeEnter(context.ExitSheet));

            return handles;
        }

        public void AfterShow(SheetShowContext context)
        {
            if (context.ExitSheet != null)
                context.ExitSheet.AfterExit(context.EnterSheet);

            context.EnterSheet.AfterEnter(context.ExitSheet);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterShow(context.EnterSheet, context.ExitSheet);
        }

        public IEnumerable<AsyncProcessHandle> BeforeHide(SheetHideContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforeHide(context.ExitSheet);
            
            var handles = new List<AsyncProcessHandle>
            {
                context.ExitSheet.BeforeExit(null)
            };

            return handles;
        }

        public void AfterHide(SheetHideContext context)
        {
            context.ExitSheet.AfterExit(null);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterHide(context.ExitSheet);
        }
    }
} 