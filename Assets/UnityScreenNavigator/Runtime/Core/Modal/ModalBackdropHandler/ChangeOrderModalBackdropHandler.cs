using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    /// <summary>
    ///     Implementation of <see cref="IModalBackdropHandler" /> that changes the drawing order of a single backdrop and
    ///     reuses it
    /// </summary>
    internal sealed class ChangeOrderModalBackdropHandler : IModalBackdropHandler
    {
        public enum ChangeTiming
        {
            BeforeAnimation,
            AfterAnimation
        }

        private readonly ChangeTiming _changeTiming;

        private readonly ModalBackdrop _prefab;
        private ModalBackdrop _instance;

        public ChangeOrderModalBackdropHandler(ModalBackdrop prefab, ChangeTiming changeTiming)
        {
            _prefab = prefab;
            _changeTiming = changeTiming;
        }

        public AsyncProcessHandle BeforeModalEnter(Modal modal, bool playAnimation)
        {
            var parent = (RectTransform)modal.transform.parent;
            var modalSiblingIndex = modal.transform.GetSiblingIndex();

            // If it is the first modal, generate a new backdrop
            if (modalSiblingIndex == 0)
            {
                var backdrop = Object.Instantiate(_prefab);
                backdrop.Setup(parent);
                backdrop.transform.SetSiblingIndex(0);
                _instance = backdrop;
                return backdrop.Enter(playAnimation);
            }

            // For the second and subsequent modals, change the drawing order of the backdrop
            var backdropSiblingIndex = modalSiblingIndex - 1;
            if (_changeTiming == ChangeTiming.BeforeAnimation)
                _instance.transform.SetSiblingIndex(backdropSiblingIndex);

            return AsyncProcessHandle.Completed();
        }

        public void AfterModalEnter(Modal modal, bool playAnimation)
        {
            var modalSiblingIndex = modal.transform.GetSiblingIndex();
            var backdropSiblingIndex = modalSiblingIndex - 1;
            // For the second and subsequent modals, change the drawing order of the backdrop
            if (_changeTiming == ChangeTiming.AfterAnimation)
                _instance.transform.SetSiblingIndex(backdropSiblingIndex);
        }

        public AsyncProcessHandle BeforeModalExit(Modal modal, bool playAnimation)
        {
            var modalSiblingIndex = modal.transform.GetSiblingIndex();

            // If it is the first modal, play the backdrop animation
            if (modalSiblingIndex == 1)
                return _instance.Exit(playAnimation);

            // For the second and subsequent modals, change the drawing order of the backdrop
            if (_changeTiming == ChangeTiming.BeforeAnimation)
                _instance.transform.SetSiblingIndex(modalSiblingIndex - 2);

            return AsyncProcessHandle.Completed();
        }

        public void AfterModalExit(Modal modal, bool playAnimation)
        {
            var modalSiblingIndex = modal.transform.GetSiblingIndex();

            // If it is the first modal, remove the backdrop
            if (modalSiblingIndex == 1)
            {
                Object.Destroy(_instance.gameObject);
                return;
            }

            // For the second and subsequent modals, change the drawing order of the backdrop
            if (_changeTiming == ChangeTiming.AfterAnimation)
                _instance.transform.SetSiblingIndex(modalSiblingIndex - 2);
        }
    }
}