using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.View.Foundation;
using Demo.Subsystem.PresentationFramework;

namespace Demo.Core.Scripts.View.Loading
{
    public sealed class LoadingView : AppView<LoadingViewState>
    {
        protected override UniTask Initialize(LoadingViewState viewState)
        {
            return UniTask.CompletedTask;
        }
    }
}
