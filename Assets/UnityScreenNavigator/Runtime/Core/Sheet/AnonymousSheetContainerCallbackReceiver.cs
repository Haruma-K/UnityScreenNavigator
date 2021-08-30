using System;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public sealed class AnonymousSheetContainerCallbackReceiver : ISheetContainerCallbackReceiver
    {
        public AnonymousSheetContainerCallbackReceiver(
            Action<(Sheet enterSheet, Sheet exitSheet)> onBeforeShow = null,
            Action<(Sheet enterSheet, Sheet exitSheet)> onAfterShow = null,
            Action<Sheet> onBeforeHide = null, Action<Sheet> onAfterHide = null)
        {
            OnBeforeShow = onBeforeShow;
            OnAfterShow = onAfterShow;
            OnBeforeHide = onBeforeHide;
            OnAfterHide = onAfterHide;
        }

        public void BeforeShow(Sheet enterSheet, Sheet exitSheet)
        {
            OnBeforeShow?.Invoke((enterSheet, exitSheet));
        }

        public void AfterShow(Sheet enterSheet, Sheet exitSheet)
        {
            OnAfterShow?.Invoke((enterSheet, exitSheet));
        }

        public void BeforeHide(Sheet exitSheet)
        {
            OnBeforeHide?.Invoke(exitSheet);
        }

        public void AfterHide(Sheet exitSheet)
        {
            OnAfterHide?.Invoke(exitSheet);
        }

        public event Action<(Sheet enterSheet, Sheet exitSheet)> OnBeforeShow;
        public event Action<(Sheet enterSheet, Sheet exitSheet)> OnAfterShow;
        public event Action<Sheet> OnBeforeHide;
        public event Action<Sheet> OnAfterHide;
    }
}