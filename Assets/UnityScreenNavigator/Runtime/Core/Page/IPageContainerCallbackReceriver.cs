namespace UnityScreenNavigator.Runtime.Core.Page
{
    public interface IPageContainerCallbackReceiver
    {
        void BeforePush(Page enterPage, Page exitPage);

        void AfterPush(Page enterPage, Page exitPage);

        void BeforePop(Page enterPage, Page exitPage);

        void AfterPop(Page enterPage, Page exitPage);
    }
}