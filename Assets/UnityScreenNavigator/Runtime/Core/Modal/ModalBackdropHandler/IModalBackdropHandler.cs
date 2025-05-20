using UnityScreenNavigator.Runtime.Foundation;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal interface IModalBackdropHandler
    {
        AsyncStatus BeforeModalEnter(Modal modal, int modalIndex, bool playAnimation);

        void AfterModalEnter(Modal modal, int modalIndex, bool playAnimation);

        AsyncStatus BeforeModalExit(Modal modal, int modalIndex, bool playAnimation);

        void AfterModalExit(Modal modal, int modalIndex, bool playAnimation);
    }
}