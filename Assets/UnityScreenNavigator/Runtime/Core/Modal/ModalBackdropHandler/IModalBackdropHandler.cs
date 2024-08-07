using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal interface IModalBackdropHandler
    {
        AsyncProcessHandle BeforeModalEnter(Modal modal, bool playAnimation);

        void AfterModalEnter(Modal modal, bool playAnimation);

        AsyncProcessHandle BeforeModalExit(Modal modal, bool playAnimation);

        void AfterModalExit(Modal modal, bool playAnimation);
    }
}