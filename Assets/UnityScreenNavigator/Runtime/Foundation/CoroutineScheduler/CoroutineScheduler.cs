using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    /// <summary>
    ///     Unity コルーチンの実行とライフサイクルを管理し、各操作に <see cref="AsyncStatusHandle" /> を提供します。
    /// </summary>
    public class CoroutineScheduler : MonoBehaviour
    {
        private static CoroutineScheduler _instance;
        private readonly Dictionary<int, RunningCoroutineInfo> _runningCoroutines = new();
        private int _nextId;

        public static CoroutineScheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    var gameObj = new GameObject($"{nameof(UnityScreenNavigator)}.{nameof(CoroutineScheduler)}");
                    gameObj.hideFlags = HideFlags.HideAndDontSave;
                    DontDestroyOnLoad(gameObj);
                    _instance = gameObj.AddComponent<CoroutineScheduler>();
                }

                return _instance;
            }
        }

        private void OnDestroy()
        {
            // スケジューラが破棄される際に、管理されていない状態で実行され続けないように、
            // 実行中のすべてのコルーチンを停止する。
            // _runningCoroutines.Values のコピーに対して反復処理を行う
            foreach (var entry in new List<RunningCoroutineInfo>(_runningCoroutines.Values))
                try
                {
                    Stop(entry.Handle);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(
                        $"CoroutineScheduler.OnDestroy 中にコルーチン (Id: {entry.Handle.CoroutineId}) の停止エラー: {ex.Message}");
                }

            _runningCoroutines.Clear();
            _instance = null;
        }

        /// <summary>
        ///     新しいコルーチンを開始します。
        /// </summary>
        /// <param name="routine">実行するコルーチンを表す IEnumerator。</param>
        /// <param name="logException">true の場合、コルーチンからキャッチされた例外は Debug.LogException を介して Unity コンソールにログ出力されます。</param>
        public AsyncStatusHandle Run(IEnumerator routine, bool logException = true)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine));

            var id = _nextId++;
            var handle = new AsyncStatusHandle(id);
            var unityCoroutine = StartCoroutine(WrapRoutine(routine, handle, logException));
            _runningCoroutines.Add(id, new RunningCoroutineInfo(unityCoroutine, handle));
            return handle;
        }

        /// <summary>
        ///     実行中のコルーチンを停止します。
        /// </summary>
        /// <param name="handle">停止するコルーチン操作。</param>
        /// <exception cref="ArgumentNullException"><paramref name="handle" /> が null の場合にスローされます。</exception>
        /// <exception cref="ArgumentException">提供された操作が認識されない、このスケジューラによって開始されなかった、または予期しない型である場合にスローされます。</exception>
        /// <exception cref="InvalidOperationException">コルーチンが既に完了している（成功、失敗、または以前の停止呼び出しによる）場合にスローされます。</exception>
        public void Stop(AsyncStatusHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));

            if (handle.CoroutineId == -1)
                throw new ArgumentException("コルーチンは実行されていません。", nameof(handle));

            var id = handle.CoroutineId;

            if (_runningCoroutines.TryGetValue(id, out var info))
            {
                // 停止前にステータスを確認（StopCoroutineが即座に完了処理を引き起こす可能性があるため）
                if (handle.IsCompleted)
                    throw new InvalidOperationException($"コルーチン (Id: {id}) は既に完了しており、再度停止できません。");

                StopCoroutine(info.UnityCoroutine);
                _runningCoroutines.Remove(id);

                // OperationCanceledException でフォルト状態にするのは、
                // StopCoroutine 呼び出し中にコルーチン自身が完了しなかった場合のみ
                // (例: try-finally で完了処理がある場合など)
                if (!handle.IsCompleted)
                    handle.MarkFaulted(
                        new OperationCanceledException(
                            $"コルーチン (Id: {id}) は Stop メソッド経由で停止されました。"));
            }
            else
            {
                // _runningCoroutines にない場合、既に完了したかどうかを確認
                if (handle.IsCompleted)
                    throw new InvalidOperationException($"コルーチン (Id: {id}) は既に完了しており、停止できません。");

                // 未完了で _runningCoroutines にも存在しない場合、不明または古いハンドル
                throw new ArgumentException($"コルーチン (Id: {id}) が見つからないか、現在実行されていません。", nameof(handle));
            }
        }

        private IEnumerator WrapRoutine(
            IEnumerator routine,
            AsyncStatusHandle handle,
            bool logExceptionToConsole
        )
        {
            try
            {
                while (true)
                {
                    object current;
                    try
                    {
                        if (!routine.MoveNext())
                            break;
                        current = routine.Current;
                    }
                    catch (Exception e)
                    {
                        // Stop() が呼び出された場合を考慮し、ステータスの上書きは行われないようにする
                        if (!handle.IsCompleted)
                            handle.MarkFaulted(e);

                        if (logExceptionToConsole)
                            Debug.LogException(e);

                        yield break;
                    }

                    yield return current;
                }

                // Stop() が呼び出された場合を考慮し、ステータスの上書きは行われないようにする
                if (!handle.IsCompleted)
                    handle.MarkCompleted();
            }
            finally
            {
                _runningCoroutines.Remove(handle.CoroutineId);
            }
        }

        private class RunningCoroutineInfo
        {
            public RunningCoroutineInfo(UnityEngine.Coroutine unityCoroutine, AsyncStatusHandle handle)
            {
                UnityCoroutine = unityCoroutine;
                Handle = handle;
            }

            public UnityEngine.Coroutine UnityCoroutine { get; }
            public AsyncStatusHandle Handle { get; }
        }
    }
}