using UnityEngine;
using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    /// <summary>
    ///     en: Implementation of <see cref="IModalBackdropHandler" /> that generates a backdrop for each modal
    /// </summary>
    internal sealed class GeneratePerModalModalBackdropHandler : IModalBackdropHandler
    {
        private readonly ModalBackdrop _prefab;

        public GeneratePerModalModalBackdropHandler(ModalBackdrop prefab)
        {
            _prefab = prefab;
        }

        public AsyncProcessHandle BeforeModalEnter(Modal modal, bool playAnimation)
        {
            var parent = (RectTransform)modal.transform.parent;
            var siblingIndex = modal.transform.GetSiblingIndex();
            var backdrop = Object.Instantiate(_prefab);
            backdrop.Setup(parent);
            backdrop.transform.SetSiblingIndex(siblingIndex);
            return backdrop.Enter(playAnimation);
        }

        public void AfterModalEnter(Modal modal, bool playAnimation)
        {
        }

        public AsyncProcessHandle BeforeModalExit(Modal modal, bool playAnimation)
        {
            var modalSiblingIndex = modal.transform.GetSiblingIndex();
            var backdropSiblingIndex = modalSiblingIndex - 1;
            var backdrop = modal.transform.parent.GetChild(backdropSiblingIndex).GetComponent<ModalBackdrop>();
            return backdrop.Exit(playAnimation);
        }

        public void AfterModalExit(Modal modal, bool playAnimation)
        {
            var modalSiblingIndex = modal.transform.GetSiblingIndex();
            var backdropSiblingIndex = modalSiblingIndex - 1;
            var backdrop = modal.transform.parent.GetChild(backdropSiblingIndex).GetComponent<ModalBackdrop>();
            Object.Destroy(backdrop.gameObject);
        }
    }
}