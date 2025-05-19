using System;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal readonly struct YieldInstructionResult
    {
        public YieldInstructionActionType ActionType { get; }
        public Exception ExceptionToReport { get; } // ActionTypeがFaultCoroutineの場合のみ関連

        private YieldInstructionResult(YieldInstructionActionType actionType, Exception exception = null)
        {
            ActionType = actionType;
            ExceptionToReport = exception;
        }

        public static YieldInstructionResult KeepRunningInCurrentTick()
        {
            return new YieldInstructionResult(YieldInstructionActionType.KeepRunningInCurrentTick);
        }

        public static YieldInstructionResult PauseForNextTick()
        {
            return new YieldInstructionResult(YieldInstructionActionType.PauseForNextTick);
        }

        public static YieldInstructionResult FaultCoroutine(Exception ex)
        {
            return new YieldInstructionResult(YieldInstructionActionType.FaultCoroutine, ex);
        }
    }
}