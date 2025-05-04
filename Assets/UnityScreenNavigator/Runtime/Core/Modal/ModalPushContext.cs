using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal readonly struct ModalPushContext
    {
        public string ModalId { get; }
        public Modal EnterModal { get; }

        public string ExitModalId { get; }
        public Modal ExitModal { get; }

        public int EnterModalIndex { get; }

        private ModalPushContext(
            string modalId,
            Modal enterModal,
            string exitModalId,
            Modal exitModal,
            int enterModalIndex
        )
        {
            ModalId = modalId;
            EnterModal = enterModal;
            ExitModalId = exitModalId;
            ExitModal = exitModal;
            EnterModalIndex = enterModalIndex;
        }

        public static ModalPushContext Create(
            string modalId,
            Modal enterModal,
            List<string> orderedModalIds,
            Dictionary<string, Modal> modals
        )
        {
            var hasExit = orderedModalIds.Count > 0;
            var exitId = hasExit ? orderedModalIds[orderedModalIds.Count - 1] : null;
            var exitModal = hasExit ? modals[exitId] : null;

            var enterIndex = modals.Count;

            return new ModalPushContext(modalId, enterModal, exitId, exitModal, enterIndex);
        }
    }
}