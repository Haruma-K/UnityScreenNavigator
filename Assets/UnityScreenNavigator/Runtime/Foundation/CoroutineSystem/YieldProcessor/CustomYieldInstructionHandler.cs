using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal class CustomYieldInstructionHandler : IYieldInstructionHandler
    {
        public bool TryHandle(object yieldInstruction, out YieldInstructionResult result)
        {
            if (yieldInstruction is CustomYieldInstruction customYield)
            {
                if (customYield.keepWaiting)
                {
                    // まだ待機が必要な場合、一時停止
                    result = YieldInstructionResult.PauseForNextTick();
                    return true;
                }

                // 待機が完了した場合、同じTickで続行
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