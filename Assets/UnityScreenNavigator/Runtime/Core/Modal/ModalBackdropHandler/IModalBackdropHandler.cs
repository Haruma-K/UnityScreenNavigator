using UnityScreenNavigator.Runtime.Foundation.Coroutine;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal interface IModalBackdropHandler
    {
        AsyncProcessHandle BeforeModalEnter(Modal modal, int modalIndex, bool playAnimation);

        void AfterModalEnter(Modal modal, int modalIndex, bool playAnimation);

        AsyncProcessHandle BeforeModalExit(Modal modal, int modalIndex, bool playAnimation);

        void AfterModalExit(Modal modal, int modalIndex, bool playAnimation);
    }
}