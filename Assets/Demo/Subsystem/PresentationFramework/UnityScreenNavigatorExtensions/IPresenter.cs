using System;

namespace Demo.Subsystem.PresentationFramework.UnityScreenNavigatorExtensions
{
    public interface IPresenter : IDisposable
    {
        bool IsDisposed { get; }
        bool IsInitialized { get; }
        void Initialize();
    }
}
