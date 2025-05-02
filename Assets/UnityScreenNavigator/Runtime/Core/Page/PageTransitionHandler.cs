using System;
using System.Linq;
using UnityEngine.Assertions;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Shared;
using UnityScreenNavigator.Runtime.Core.Sheet;

namespace UnityScreenNavigator.Runtime.Core.Page
{
    public sealed class PageTransitionHandler
    {
        private readonly PageContainer _container;

        public PageTransitionHandler(PageContainer container)
        {
            Assert.IsNotNull(container);
            _container = container;
        }

        public bool IsInTransition { get; private set; }

        public void Begin()
        {
            if (IsInTransition)
                throw new InvalidOperationException("Transition already in progress.");

            IsInTransition = true;

            if (UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
                return;

            if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
            {
                // 他のコンテナがトランジション中の場合は、すでにそのコンテナの開始処理でSetAllContainersInteractableが実行されているので、ここでは何もしない
                if (!AllContainersFinishedTransition())
                    return;

                SetAllContainersInteractable(false);
            }
            else
            {
                SetSelfContainersInteractable(false);
            }
        }

        public void End()
        {
            if (!IsInTransition)
                throw new InvalidOperationException("Transition has not started.");

            IsInTransition = false;

            if (UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
                return;

            if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
            {
                // 他のコンテナがトランジション中の場合は、そのコンテナの終了処理でSetAllContainersInteractableが実行されるので、ここでは何もしない
                if (!AllContainersFinishedTransition())
                    return;

                SetAllContainersInteractable(true);
            }
            else
            {
                SetSelfContainersInteractable(true);
            }
        }

        private bool AllContainersFinishedTransition()
        {
            return PageContainer.Instances.All(x => !x.IsInTransition)
                   && ModalContainer.Instances.All(x => !x.IsInTransition)
                   && SheetContainer.Instances.All(x => !x.IsInTransition);
        }

        private void SetAllContainersInteractable(bool value)
        {
            foreach (var container in PageContainer.Instances)
                container.Interactable = value;
            foreach (var container in ModalContainer.Instances)
                container.Interactable = value;
            foreach (var container in SheetContainer.Instances)
                container.Interactable = value;
        }

        private void SetSelfContainersInteractable(bool value)
        {
            _container.Interactable = value;
        }
    }
}