namespace UnityScreenNavigator.Runtime.Foundation
{
    internal interface IYieldInstructionHandler
    {
        /// <summary>
        ///     指定されたyieldInstructionをこのハンドラが処理できるか試みます。
        /// </summary>
        /// <param name="yieldInstruction">処理対象のyieldされたオブジェクト。</param>
        /// <param name="result">処理結果。このハンドラがyieldInstructionを処理した場合に設定されます。</param>
        /// <returns>このハンドラがyieldInstructionを処理した場合はtrue、それ以外はfalse。</returns>
        bool TryHandle(object yieldInstruction, out YieldInstructionResult result);

        /// <summary>
        ///     指定されたyieldInstructionに関連する内部状態をクリーンアップします。
        ///     このハンドラがその命令を管理していない場合は何もしません。
        /// </summary>
        void CleanupInstructionState(object yieldInstruction);
    }
}