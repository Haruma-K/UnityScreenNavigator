using System.Collections;
using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal sealed class CoroutineRunner
    {
        private readonly Queue<CoroutineHandle> _runningHandles = new();

        public AsyncStatus Run(IEnumerator routine)
        {
            var handle = new CoroutineHandle(routine);
            _runningHandles.Enqueue(handle);
            return handle.Status;
        }

        /// <summary>
        ///     登録された全てのコルーチンを1ステップ進めます。
        ///     完了・エラーになったものはスケジュールから除外されます。
        /// </summary>
        public void Tick()
        {
            var count = _runningHandles.Count;
            for (var i = 0; i < count; i++)
            {
                var handle = _runningHandles.Dequeue();
                var result = handle.Step();

                if (result == CoroutineStepResult.Continue)
                    _runningHandles.Enqueue(handle);
            }
        }
    }
}