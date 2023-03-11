using System.Threading.Tasks;
using UnityEngine.Assertions;
using UnityScreenNavigator.Runtime.Core.Modal;

namespace Demo.Subsystem.PresentationFramework
{
    public abstract class Modal<TRootView, TViewState> : Modal
        where TRootView : AppView<TViewState>
        where TViewState : AppViewState
    {
        public TRootView root;
        private bool _isInitialized;
        private TViewState _state;

        protected virtual ViewInitializationTiming RootInitializationTiming =>
            ViewInitializationTiming.BeforeFirstEnter;

        public void Setup(TViewState state)
        {
            Assert.IsNotNull(root);
            _state = state;
        }

#if USN_USE_ASYNC_METHODS
        public override async Task Initialize()
        {
            Assert.IsNotNull(root);

            await base.Initialize();

            if (RootInitializationTiming == ViewInitializationTiming.Initialize && !_isInitialized)
            {
                await root.InitializeAsync(_state);
                _isInitialized = true;
            }
        }
#else
        public override IEnumerator Initialize()
        {
            Assert.IsNotNull(root);

            yield return base.Initialize();

            if (RootInitializationTiming == ViewInitializationTiming.Initialize && !_isInitialized)
            {
                yield return root.InitializeAsync(_state).ToCoroutine();
                _isInitialized = true;
            }
        }
#endif

#if USN_USE_ASYNC_METHODS
        public override async Task WillPushEnter()
        {
            Assert.IsNotNull(root);

            await base.WillPushEnter();

            if (RootInitializationTiming == ViewInitializationTiming.BeforeFirstEnter && !_isInitialized)
            {
                await root.InitializeAsync(_state);
                _isInitialized = true;
            }
        }
#else
        public override IEnumerator WillPushEnter()
        {
            Assert.IsNotNull(root);

            yield return base.WillPushEnter();

            if (RootInitializationTiming == ViewInitializationTiming.BeforeFirstEnter && !_isInitialized)
            {
                yield return root.InitializeAsync(_state).ToCoroutine();
                _isInitialized = true;
            }
        }
#endif
    }
}
