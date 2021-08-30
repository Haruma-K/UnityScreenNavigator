using System;
using UnityScreenNavigator.Runtime.Core.Shared;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public static class ModalContainerExtensions
    {
        /// <summary>
        ///     Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(this ModalContainer self,
            Action<(Modal enterModal, Modal exitModal)> onBeforePush = null,
            Action<(Modal enterModal, Modal exitModal)> onAfterPush = null,
            Action<(Modal enterModal, Modal exitModal)> onBeforePop = null,
            Action<(Modal enterModal, Modal exitModal)> onAfterPop = null)
        {
            var callbackReceiver =
                new AnonymousModalContainerCallbackReceiver(onBeforePush, onAfterPush, onBeforePop, onAfterPop);
            self.AddCallbackReceiver(callbackReceiver);
        }

        /// <summary>
        ///     Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="modal"></param>
        /// <param name="onBeforePush"></param>
        /// <param name="onAfterPush"></param>
        /// <param name="onBeforePop"></param>
        /// <param name="onAfterPop"></param>
        public static void AddCallbackReceiver(this ModalContainer self, Modal modal,
            Action<Modal> onBeforePush = null, Action<Modal> onAfterPush = null,
            Action<Modal> onBeforePop = null, Action<Modal> onAfterPop = null)
        {
            var callbackReceiver = new AnonymousModalContainerCallbackReceiver();
            callbackReceiver.OnBeforePush += x =>
            {
                var (enterModal, exitModal) = x;
                if (enterModal.Equals(modal))
                {
                    onBeforePush?.Invoke(exitModal);
                }
            };
            callbackReceiver.OnAfterPush += x =>
            {
                var (enterModal, exitModal) = x;
                if (enterModal.Equals(modal))
                {
                    onAfterPush?.Invoke(exitModal);
                }
            };
            callbackReceiver.OnBeforePop += x =>
            {
                var (enterModal, exitModal) = x;
                if (exitModal.Equals(modal))
                {
                    onBeforePop?.Invoke(enterModal);
                }
            };
            callbackReceiver.OnAfterPop += x =>
            {
                var (enterModal, exitModal) = x;
                if (exitModal.Equals(modal))
                {
                    onAfterPop?.Invoke(enterModal);
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