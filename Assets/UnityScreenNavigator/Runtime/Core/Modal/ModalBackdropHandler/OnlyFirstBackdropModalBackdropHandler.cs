using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;
using Object = UnityEngine.Object;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    /// <summary>
    ///     Implementation of <see cref="IModalBackdropHandler" /> that generates a backdrop only for the first modal
    /// </summary>
    internal sealed class OnlyFirstBackdropModalBackdropHandler : IModalBackdropHandler
    {
        private readonly ModalBackdrop _prefab;

        public OnlyFirstBackdropModalBackdropHandler(ModalBackdrop prefab)
        {
            _prefab = prefab;
        }

        public AsyncProcessHandle BeforeModalEnter(Modal modal, bool playAnimation)
        {
            var parent = (RectTransform)modal.transform.parent;
            var modalSiblingIndex = modal.transform.GetSiblingIndex();

            // Do not generate a backdrop for the first modal
            if (modalSiblingIndex != 0)
                return AsyncProcessHandle.Completed();

            var backdrop = Object.Instantiate(_prefab);
            backdrop.Setup(parent);
            backdrop.transform.SetSiblingIndex(0);
            return backdrop.Enter(playAnimation);
        }

        public void AfterModalEnter(Modal modal, bool playAnimation)
        {
        }

        public AsyncProcessHandle BeforeModalExit(Modal modal, bool playAnimation)
        {
            var modalSiblingIndex = modal.transform.GetSiblingIndex();

            // Do not remove the backdrop for the first modal
            if (modalSiblingIndex != 1)
                return AsyncProcessHandle.Completed();

            var backdrop = modal.transform.parent.GetChild(0).GetComponent<ModalBackdrop>();
            return backdrop.Exit(playAnimation);
        }

        public void AfterModalExit(Modal modal, bool playAnimation)
        {
            var modalSiblingIndex = modal.transform.GetSiblingIndex();

            // Do not remove the backdrop for the first modal
            if (modalSiblingIndex != 1)
                return;

            var backdrop = modal.transform.parent.GetChild(0).GetComponent<ModalBackdrop>();
            Object.Destroy(backdrop.gameObject);
        }
    }
}