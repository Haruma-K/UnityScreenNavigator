namespace UnityScreenNavigator.Runtime.Foundation
{
    internal enum YieldInstructionActionType
    {
        /// <summary>
        ///     CoroutineHandleのメインループを継続し、同じTick内で次のMoveNextを試みます。
        /// </summary>
        KeepRunningInCurrentTick,

        /// <summary>
        ///     CoroutineHandleの現在のTickの処理を一時停止し、CoroutineStepResult.Continueを返します。
        /// </summary>
        PauseForNextTick,

        /// <summary>
        ///     CoroutineHandleを失敗状態にし、CoroutineStepResult.Faultedを返します。
        /// </summary>
        FaultCoroutine
    }
}