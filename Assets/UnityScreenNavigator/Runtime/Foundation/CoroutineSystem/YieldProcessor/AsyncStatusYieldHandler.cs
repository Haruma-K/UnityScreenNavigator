namespace UnityScreenNavigator.Runtime.Foundation
{
    internal class AsyncStatusYieldHandler : IYieldInstructionHandler
    {
        public bool TryHandle(object yieldInstruction, out YieldInstructionResult result)
        {
            if (yieldInstruction is AsyncStatus awaitedStatus)
            {
                if (!awaitedStatus.IsCompleted)
                {
                    result = YieldInstructionResult.PauseForNextTick(); // 完了していないので待機
                    return true;
                }

                if (awaitedStatus.IsFaulted)
                {
                    // 依存するAsyncStatusが失敗した場合、このコルーチンも失敗させる
                    result = YieldInstructionResult.FaultCoroutine(awaitedStatus.Exception);
                    return true;
                }

                // 依存するAsyncStatusが正常に完了した場合、同じTickで続行
                result = YieldInstructionResult.KeepRunningInCurrentTick();
                return true;
            }

            result = default; // このハンドラでは処理できない
            return false;
        }

        public void CleanupInstructionState(object yieldInstruction)
        {
        }
    }
}