using System;

namespace UnityScreenNavigator.Runtime.Core.Sheet
{
    public static class SheetContainerExtensions
    {
        /// <summary>
        ///     Add callbacks.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="onBeforeShow"></param>
        /// <param name="onAfterShow"></param>
        /// <param name="onBeforeHide"></param>
        /// <param name="onAfterHide"></param>
        public static void AddCallbackReceiver(this SheetContainer self,
            Action<(Sheet enterSheet, Sheet exitSheet)> onBeforeShow = null,
            Action<(Sheet enterSheet, Sheet exitSheet)> onAfterShow = null,
            Action<Sheet> onBeforeHide = null, Action<Sheet> onAfterHide = null)
        {
            var callbackReceiver =
                new AnonymousSheetContainerCallbackReceiver(onBeforeShow, onAfterShow, onBeforeHide, onAfterHide);
            self.AddCallbackReceiver(callbackReceiver);
        }
    }
}