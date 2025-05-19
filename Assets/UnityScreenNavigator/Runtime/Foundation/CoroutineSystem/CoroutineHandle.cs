using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal sealed class CoroutineHandle
    {
        private readonly CoroutineRunner _ownerRunner;
        private readonly IEnumerator _routine;
        private readonly YieldProcessor _yieldProcessor;
        private object _lastYieldedValue;

        public CoroutineHandle(IEnumerator routine, CoroutineRunner ownerRunner)
        {
            _routine = routine;
            _ownerRunner = ownerRunner ?? throw new ArgumentNullException(nameof(ownerRunner));
            Status = new AsyncStatus();

            // YieldProcessorを初期化し、ハンドラを登録
            // ハンドラの順序は重要。より具体的なものから汎用的なものの順に。
            var handlers = new List<IYieldInstructionHandler>
            {
                new UnityTimedInstructionHandler(),
                new AsyncStatusYieldHandler(),
                new CustomYieldInstructionHandler()
                // 必要に応じて他のカスタムハンドラをここに追加
            };
            _yieldProcessor = new YieldProcessor(handlers);
        }

        public AsyncStatus Status { get; }

        /// <summary>
        ///     コルーチンを1ステップ進めます。
        ///     結果として、継続・完了・失敗のいずれかを返します。
        /// </summary>
        public CoroutineStepResult Step()
        {
            if (Status.IsFaulted)
                return CoroutineStepResult.Faulted;

            if (Status.IsCompleted)
                return CoroutineStepResult.Completed;

            try
            {
                // while (true) ループを使用して、特定のyield命令 (AsyncStatus, CustomYieldInstructionなど) が
                // 即座に解決された場合に、同じTick内でコルーチンの次のステップを試行できるようにします。
                while (true)
                {
                    if (!_routine.MoveNext())
                    {
                        PerformCleanup();
                        Status.MarkCompleted();
                        return CoroutineStepResult.Completed;
                    }

                    _lastYieldedValue = _routine.Current;

                    // yield された値が「純粋な」IEnumerator (Unityの他の YieldInstruction や CustomYieldInstruction ではない)
                    // の場合に入れ子コルーチンとして処理する。
                    if (_lastYieldedValue is IEnumerator nestedRoutine &&
                        _lastYieldedValue is not YieldInstruction && // WaitForSeconds など Unity 標準の YieldInstruction を除外
                        _lastYieldedValue is not CustomYieldInstruction) // CustomYieldInstruction から派生したものを除外
                    {
                        var nestedStatus = _ownerRunner.Run(nestedRoutine);
                        _lastYieldedValue = nestedStatus; // 親は子の AsyncStatus を待つ
                    }

                    var yieldResult = _yieldProcessor.Process(_lastYieldedValue);

                    switch (yieldResult.ActionType)
                    {
                        case YieldInstructionActionType.KeepRunningInCurrentTick:
                            // whileループを継続し、同じTick内で次のMoveNextを実行
                            continue;

                        case YieldInstructionActionType.PauseForNextTick:
                            // 今回のTickの処理を中断し、次のTickで再開
                            return CoroutineStepResult.Continue;

                        case YieldInstructionActionType.FaultCoroutine:
                            // コルーチンを失敗状態にする
                            PerformCleanup();
                            Status.MarkFaulted(yieldResult.ExceptionToReport);
                            return CoroutineStepResult.Faulted;

                        default:
                            // 到達しないはずだが、念のため
                            // (YieldProcessorが常に有効な値を返すため)
                            PerformCleanup();
                            Status.MarkFaulted(
                                new InvalidOperationException(
                                    $"Unknown YieldInstructionActionType: {yieldResult.ActionType}"));
                            return CoroutineStepResult.Faulted;
                    }
                }
            }
            catch (Exception ex)
            {
                // コルーチン実行中の予期せぬ例外
                PerformCleanup();
                Status.MarkFaulted(ex);
                return CoroutineStepResult.Faulted;
            }
        }

        public void ForceStopAndCleanup()
        {
            PerformCleanup(); // 最後にyieldしていた値に対するクリーンアップ
            if (!Status.IsCompleted)
                Status.MarkFaulted(new OperationCanceledException("Coroutine was stopped."));
        }

        private void PerformCleanup()
        {
            if (_lastYieldedValue != null && _yieldProcessor != null)
                // _yieldProcessor が持つ全ハンドラに対してクリーンアップを試みる
                // (YieldProcessor にハンドラリストを取得する口がないため、
                //  YieldProcessor自体に CleanupAllHandlers(object instruction) のようなメソッドを作るか、
                //  ハンドラリストを CoroutineHandle も保持するなどの変更が必要。
                //  ここでは、YieldProcessor にメソッドを追加する方針で考えます。)
                _yieldProcessor.CleanupHandlers(_lastYieldedValue); // ★YieldProcessor経由でクリーンアップ
            _lastYieldedValue = null; // クリーンアップ後はクリア
        }
    }
}