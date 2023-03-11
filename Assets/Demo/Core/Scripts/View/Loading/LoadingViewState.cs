using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.Loading
{
    public sealed class LoadingViewState : AppViewState, ILoadingState
    {
        protected override void DisposeInternal()
        {
        }
    }

    internal interface ILoadingState
    {
    }
}
