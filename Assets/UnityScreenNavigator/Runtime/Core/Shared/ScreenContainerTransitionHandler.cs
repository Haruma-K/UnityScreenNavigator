using System;
using System.Linq;
using UnityScreenNavigator.Runtime.Core.Modal;
using UnityScreenNavigator.Runtime.Core.Page;
using UnityScreenNavigator.Runtime.Core.Sheet;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    public sealed class ScreenContainerTransitionHandler
    {
        private readonly IScreenContainer _container;
        public bool IsInTransition { get; private set; }

        public ScreenContainerTransitionHandler(IScreenContainer container)
        {
            _container = container;
        }

        public void Begin()
        {
            if (IsInTransition)
                throw new InvalidOperationException("Transition already in progress.");

            if (UnityScreenNavigatorSettings.Instance.EnableInteractionInTransition)
            {
                IsInTransition = true;
                return;
            }

            if (UnityScreenNavigatorSettings.Instance.ControlInteractionsOfAllContainers)
            {
                if (!AllContainersFinishedTransition())
                {
                    IsInTransition = true;
                    return;
                }

                SetAllContainersInteractable(false);
            }
            else
            {
                _container.Interactable = false;
            }

            IsInTransition = true;
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
                if (!AllContainersFinishedTransition())
                    return;

                SetAllContainersInteractable(true);
            }
            else
            {
                _container.Interactable = true;
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
    }
}