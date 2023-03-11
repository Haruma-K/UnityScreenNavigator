using Demo.Subsystem.PresentationFramework;
using UniRx;

namespace Demo.Core.Scripts.View.Parts.UnitPortrait
{
    public sealed class UnitPortraitViewState : AppViewState, IUnitPortraitState
    {
        private readonly ReactiveProperty<string> _imageResourceKey = new ReactiveProperty<string>();

        public IReactiveProperty<string> ImageResourceKey => _imageResourceKey;

        protected override void DisposeInternal()
        {
            _imageResourceKey.Dispose();
        }
    }

    internal interface IUnitPortraitState
    {
    }
}