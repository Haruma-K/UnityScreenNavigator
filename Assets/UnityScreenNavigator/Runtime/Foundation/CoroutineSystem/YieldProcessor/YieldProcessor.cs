using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal class YieldProcessor
    {
        private readonly List<IYieldInstructionHandler> _handlers;

        public YieldProcessor(List<IYieldInstructionHandler> handlers)
        {
            _handlers = handlers ?? new List<IYieldInstructionHandler>();
        }

        public YieldInstructionResult Process(object yieldInstruction)
        {
            foreach (var handler in _handlers)
                if (handler.TryHandle(yieldInstruction, out var result))
                    return result; // 最初に処理できたハンドラの結果を使用

            // どの専用ハンドラも処理しなかった場合 (例: yield return null)、
            // デフォルトで次のTickまで一時停止
            return YieldInstructionResult.PauseForNextTick();
        }


        /// <summary>
        ///     登録されている全てのハンドラに対して、指定されたyieldInstructionに関連する状態のクリーンアップを試みます。
        /// </summary>
        public void CleanupHandlers(object yieldInstruction) // ★追加
        {
            if (yieldInstruction == null) return;

            foreach (var handler in _handlers)
                handler.CleanupInstructionState(yieldInstruction);
        }
    }
}