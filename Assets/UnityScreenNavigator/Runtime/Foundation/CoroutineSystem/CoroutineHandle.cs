using System;
using System.Collections;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal sealed class CoroutineHandle
    {
        private readonly IEnumerator _routine;

        public CoroutineHandle(IEnumerator routine)
        {
            _routine = routine;
            Status = new AsyncStatus();
        }

        public AsyncStatus Status { get; }

        /// <summary>
        ///     コルーチンを1ステップ進めます。
        ///     結果として、継続・完了・失敗のいずれかを返します。
        /// </summary>
        public CoroutineStepResult Step()
        {
            if (Status.IsCompleted)
                return CoroutineStepResult.Completed;

            try
            {
                // while (true) ループを使用して、AsyncStatus が解決された場合に
                // 同じTick内でコルーチンの次のステップを試行できるようにします。
                while (true)
                {
                    if (!_routine.MoveNext())
                    {
                        Status.MarkCompleted();
                        return CoroutineStepResult.Completed;
                    }

                    // 依存するAsyncStatusをyield（待機）している場合の処理
                    if (_routine.Current is AsyncStatus awaited)
                    {
                        if (!awaited.IsCompleted)
                            // 依存するAsyncStatusがまだ完了していなければ、
                            // 今回のTickではここまでとし、コルーチンは継続（引き続き待機）。
                            return CoroutineStepResult.Continue;

                        // 依存するAsyncStatusが完了している場合
                        if (awaited.IsFaulted)
                        {
                            // このコルーチンも失敗としてマークし終了
                            Status.MarkFaulted(awaited.Exception);
                            return CoroutineStepResult.Faulted;
                        }

                        // 依存するAsyncStatusが正常に完了している場合は次のステップに進む必要があるので、
                        // whileループの先頭に戻り、再度 _routine.MoveNext() を試みる。
                    }
                    else
                    {
                        // _routine.Current が AsyncStatus 以外の場合 (例: yield return null)
                        // 通常のコルーチンと同様に、今回のTickではここまでとし、コルーチンは継続。
                        return CoroutineStepResult.Continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Status.MarkFaulted(ex);
                return CoroutineStepResult.Faulted;
            }
        }
    }
}