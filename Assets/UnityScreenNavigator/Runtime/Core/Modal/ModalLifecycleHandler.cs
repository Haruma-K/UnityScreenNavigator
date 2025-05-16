using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal sealed class ModalLifecycleHandler
    {
        private readonly IModalBackdropHandler _backdropHandler;
        private readonly IEnumerable<IModalContainerCallbackReceiver> _callbackReceivers;
        private readonly RectTransform _containerTransform;

        public ModalLifecycleHandler(
            RectTransform containerTransform,
            IEnumerable<IModalContainerCallbackReceiver> callbackReceivers,
            IModalBackdropHandler backdropHandler
        )
        {
            _containerTransform = containerTransform;
            _callbackReceivers = callbackReceivers;
            _backdropHandler = backdropHandler;
        }

        public IEnumerator AfterLoad(ModalPushContext context)
        {
            yield return context.EnterModal.AfterLoad(_containerTransform);
        }

        public IEnumerator BeforePush(ModalPushContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePush(context.EnterModal, context.ExitModal);

            var handles = new List<AsyncProcessHandle>();
            if (context.ExitModal != null)
                handles.Add(context.ExitModal.BeforeExit(true, context.EnterModal));

            handles.Add(context.EnterModal.BeforeEnter(true, context.ExitModal));
            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public IEnumerator Push(ModalPushContext context, bool playAnimation)
        {
            var handles = new List<AsyncProcessHandle>
            {
                _backdropHandler.BeforeModalEnter(context.EnterModal, context.EnterModalIndex, playAnimation)
            };

            if (context.ExitModal != null)
                handles.Add(context.ExitModal.Exit(true, playAnimation, context.EnterModal));

            handles.Add(context.EnterModal.Enter(true, playAnimation, context.ExitModal));
            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;

            _backdropHandler.AfterModalEnter(context.EnterModal, context.EnterModalIndex, true);
        }

        public void AfterPush(ModalPushContext context)
        {
            if (context.ExitModal != null)
                context.ExitModal.AfterExit(true, context.EnterModal);

            context.EnterModal.AfterEnter(true, context.ExitModal);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPush(context.EnterModal, context.ExitModal);
        }

        public IEnumerator BeforePop(ModalPopContext context)
        {
            foreach (var receiver in _callbackReceivers)
                receiver.BeforePop(context.EnterModal, context.FirstExitModal);

            var handles = new List<AsyncProcessHandle>();
            foreach (var exitModal in context.ExitModals)
                handles.Add(exitModal.BeforeExit(false, context.EnterModal));

            if (context.EnterModal != null)
                handles.Add(context.EnterModal.BeforeEnter(false, context.FirstExitModal));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }

        public IEnumerator Pop(ModalPopContext context, bool playAnimation)
        {
            var handles = new List<AsyncProcessHandle>();

            for (var i = 0; i < context.ExitModals.Count; i++)
            {
                var exitModal = context.ExitModals[i];
                var exitModalIndex = context.ExitModalIndices[i];
                var partner = i == context.ExitModals.Count - 1 ? context.EnterModal : context.ExitModals[i + 1];

                handles.Add(_backdropHandler.BeforeModalExit(exitModal, exitModalIndex, playAnimation));
                handles.Add(exitModal.Exit(false, playAnimation, partner));
            }

            if (context.EnterModal != null)
                handles.Add(context.EnterModal.Enter(false, playAnimation, context.FirstExitModal));

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;

            for (var i = 0; i < context.ExitModals.Count; i++)
            {
                var exitModal = context.ExitModals[i];
                var exitModalIndex = context.ExitModalIndices[i];
                _backdropHandler.AfterModalExit(exitModal, exitModalIndex, playAnimation);
            }
        }

        public void AfterPop(ModalPopContext context)
        {
            foreach (var exitModal in context.ExitModals)
                exitModal.AfterExit(false, context.EnterModal);
            if (context.EnterModal != null)
                context.EnterModal.AfterEnter(false, context.FirstExitModal);

            foreach (var receiver in _callbackReceivers)
                receiver.AfterPop(context.EnterModal, context.FirstExitModal);
        }

        public IEnumerator AfterPopRoutine(ModalPopContext context)
        {
            var handles = context.ExitModals.Select(exitModal => exitModal.BeforeRelease());

            foreach (var handle in handles)
                while (!handle.IsTerminated)
                    yield return handle;
        }
    }
}