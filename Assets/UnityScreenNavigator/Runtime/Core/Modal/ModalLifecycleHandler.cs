using System.Collections.Generic;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal sealed class ModalLifecycleHandler
    {
        private readonly IEnumerable<IModalContainerCallbackReceiver> _callbackReceivers;

        public ModalLifecycleHandler(IEnumerable<IModalContainerCallbackReceiver> callbackReceivers)
        {
            _callbackReceivers = callbackReceivers;
        }

        public IEnumerable<AsyncProcessHandle> BeforePush(ModalPushContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePush(context.EnterModal, context.ExitModal);

            var handles = new List<AsyncProcessHandle>();
            if (context.ExitModal != null)
                handles.Add(context.ExitModal.BeforeExit(true, context.EnterModal));

            handles.Add(context.EnterModal.BeforeEnter(true, context.ExitModal));

            return handles;
        }

        public void AfterPush(ModalPushContext context)
        {
            if (context.ExitModal != null)
                context.ExitModal.AfterExit(true, context.EnterModal);

            context.EnterModal.AfterEnter(true, context.ExitModal);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPush(context.EnterModal, context.ExitModal);
        }

        public IEnumerable<AsyncProcessHandle> BeforePop(ModalPopContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePop(context.EnterModal, context.FirstExitModal);

            var handles = new List<AsyncProcessHandle>
            {
                context.FirstExitModal.BeforeExit(false, context.EnterModal)
            };

            if (context.EnterModal != null)
                handles.Add(context.EnterModal.BeforeEnter(false, context.FirstExitModal));

            return handles;
        }

        public void AfterPop(ModalPopContext context)
        {
            context.FirstExitModal.AfterExit(false, context.EnterModal);
            if (context.EnterModal != null)
                context.EnterModal.AfterEnter(false, context.FirstExitModal);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPop(context.EnterModal, context.FirstExitModal);
        }
    }
}