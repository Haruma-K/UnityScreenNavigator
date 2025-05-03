namespace UnityScreenNavigator.Runtime.Core.Shared
{
    public interface IScreenContainer
    {
        bool IsInTransition { get; }

        bool Interactable { get; set; }
    }
}