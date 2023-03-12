using Demo.Core.Scripts.View.Loading;
using Demo.Development.View.Shared;

namespace Demo.Development.View.Loading
{
    public sealed class LoadingDevelopment : AppViewDevelopment<LoadingView, LoadingViewState>
    {
        protected override LoadingViewState CreateState()
        {
            var state = new LoadingViewState();
            return state;
        }
    }
}
