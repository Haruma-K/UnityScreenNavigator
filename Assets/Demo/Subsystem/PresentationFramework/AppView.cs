using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Demo.Subsystem.PresentationFramework
{
    public abstract class AppView<TState> : MonoBehaviour
        where TState : AppViewState
    {
        private bool _isInitialized;

        public async UniTask InitializeAsync(TState state)
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            await Initialize(state);
        }

        protected abstract UniTask Initialize(TState state);
    }
}
