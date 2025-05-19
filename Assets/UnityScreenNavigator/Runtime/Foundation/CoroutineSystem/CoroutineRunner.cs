using System.Collections;
using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal sealed class CoroutineRunner
    {
        private readonly List<CoroutineHandle> _runningHandles = new();
        private readonly Dictionary<AsyncStatus, CoroutineHandle> _statusToHandleMap = new();

        public AsyncStatus Run(IEnumerator routine)
        {
            var handle = new CoroutineHandle(routine, this);
            _runningHandles.Add(handle);
            _statusToHandleMap[handle.Status] = handle;
            return handle.Status;
        }

        /// <summary>
        ///     登録された全てのコルーチンを1ステップ進めます。
        ///     完了・エラーになったものはスケジュールから除外されます。
        /// </summary>
        public void Tick()
        {
            // リストを逆順に処理することで、処理中に要素が削除されてもインデックスがずれないようにする
            for (var i = _runningHandles.Count - 1; i >= 0; i--)
            {
                if (i >= _runningHandles.Count)
                    continue; // 他の処理でリストの末尾が変更された場合の対策

                var handle = _runningHandles[i];
                var result = handle.Step();

                if (result != CoroutineStepResult.Continue)
                {
                    _runningHandles.RemoveAt(i); // 完了またはエラーになったものをリストから削除
                    _statusToHandleMap.Remove(handle.Status); // マップからも削除
                }
            }
        }

        /// <summary>
        ///     指定されたAsyncStatusに関連するコルーチンを停止します。
        /// </summary>
        /// <param name="statusToStop">停止対象コルーチンのAsyncStatus。</param>
        public void Stop(AsyncStatus statusToStop)
        {
            if (statusToStop == null) return;

            if (_statusToHandleMap.TryGetValue(statusToStop, out var handle))
                if (!handle.Status.IsCompleted)
                    handle.ForceStopAndCleanup();
        }

        /// <summary>
        ///     全ての実行中コルーチンを停止します。
        /// </summary>
        public void StopAll()
        {
            // リストのコピーに対して処理を行うか、逆順で処理する
            for (var i = _runningHandles.Count - 1; i >= 0; i--)
            {
                var handle = _runningHandles[i];
                if (!handle.Status.IsCompleted) handle.ForceStopAndCleanup();
            }
        }
    }
}