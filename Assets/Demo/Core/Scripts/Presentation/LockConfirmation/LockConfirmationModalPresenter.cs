using System.Threading.Tasks;
using Demo.Core.Scripts.Presentation.Shared;
using Demo.Core.Scripts.View.Confirmation;
using Demo.Subsystem.Misc;
using UniRx;

namespace Demo.Core.Scripts.Presentation.LockConfirmation
{
    public sealed class
        LockConfirmationModalPresenter : ModalPresenterBase<ConfirmationModal, ConfirmationView, ConfirmationViewState>
    {
        public LockConfirmationModalPresenter(ConfirmationModal view, ITransitionService transitionService)
            : base(view, transitionService)
        {
        }

        protected override async Task ViewDidLoad(ConfirmationModal view, ConfirmationViewState viewState)
        {
            // Set view state with initial values.
            viewState.Message.Value = "This feature is locked.";

            // Observe changes of view state.
            viewState.CloseButtonClicked
                .Subscribe(_ => TransitionService.PopCommandExecuted())
                .AddTo(this);
        }
    }
}