namespace UnityScreenNavigator.Runtime.Core.Modal
{
    public interface IModalContainerCallbackReceiver
    {
        void BeforePush(Modal enterModal, Modal exitModal);

        void AfterPush(Modal enterModal, Modal exitModal);

        void BeforePop(Modal enterModal, Modal exitModal);

        void AfterPop(Modal enterModal, Modal exitModal);
    }
}