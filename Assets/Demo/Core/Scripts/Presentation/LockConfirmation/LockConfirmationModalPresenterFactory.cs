using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Confirmation;

namespace Demo.Core.Scripts.Presentation.LockConfirmation
{
    public sealed class LockConfirmationModalPresenterFactory
    {
        public LockConfirmationModalPresenter Create(ConfirmationModal view, ITransitionService transitionService)
        {
            return new LockConfirmationModalPresenter(view, transitionService);
        }
    }
}