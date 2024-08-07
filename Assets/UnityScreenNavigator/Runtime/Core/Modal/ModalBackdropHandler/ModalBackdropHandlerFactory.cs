using System;

namespace UnityScreenNavigator.Runtime.Core.Modal
{
    internal static class ModalBackdropHandlerFactory
    {
        public static IModalBackdropHandler Create(ModalBackdropStrategy strategy, ModalBackdrop prefab)
        {
            return strategy switch
            {
                ModalBackdropStrategy.GeneratePerModal => new GeneratePerModalModalBackdropHandler(prefab),
                ModalBackdropStrategy.OnlyFirstBackdrop => new OnlyFirstBackdropModalBackdropHandler(prefab),
                ModalBackdropStrategy.ChangeOrderBeforeAnimation => new ChangeOrderModalBackdropHandler(
                    prefab,
                    ChangeOrderModalBackdropHandler.ChangeTiming.BeforeAnimation),
                ModalBackdropStrategy.ChangeOrderAfterAnimation => new ChangeOrderModalBackdropHandler(
                    prefab,
                    ChangeOrderModalBackdropHandler.ChangeTiming.AfterAnimation),
                _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
            };
        }
    }
}