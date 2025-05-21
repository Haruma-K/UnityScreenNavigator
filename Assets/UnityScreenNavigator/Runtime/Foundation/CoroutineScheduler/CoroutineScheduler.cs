using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal sealed class CoroutineScheduler : MonoBehaviour
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
            // スケジューラが破棄される際に、管理されていない状態で実行され続けないように、実行中のすべてのコルーチンを停止する。
            foreach (var entry in new List<RunningCoroutineInfo>(_runningCoroutines.Values))
                try
                {
                    Stop(entry.Handle);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(
                        $"Error while stopping coroutine (Id: {entry.Handle.CoroutineId}) during CoroutineScheduler.OnDestroy: {ex.Message}");
                }

            _runningCoroutines.Clear();
            _instance = null;
        }

        public static AsyncProcessHandle Run(IEnumerator routine, bool logException = true)
        {
            return Instance.RunInternal(routine, logException);
        }

        private AsyncProcessHandle RunInternal(IEnumerator routine, bool logException = true)
        {
            if (routine == null)
                throw new ArgumentNullException(nameof(routine), "The coroutine routine cannot be null.");

            var id = _nextId++;
            var handle = new AsyncProcessHandle(id);
            var unityCoroutine = StartCoroutine(WrapRoutine(routine, handle, logException));
            _runningCoroutines.Add(id, new RunningCoroutineInfo(unityCoroutine, handle));
            return handle;
        }

        public void Stop(AsyncProcessHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle), "The coroutine handle cannot be null.");

            var id = handle.CoroutineId;

            if (_runningCoroutines.TryGetValue(id, out var info))
            {
                if (handle.IsCompleted)
                    throw new InvalidOperationException(
                        $"Coroutine (Id: {id}) has already completed and cannot be stopped again.");

                StopCoroutine(info.UnityCoroutine);
                _runningCoroutines.Remove(id);

                if (!handle.IsCompleted)
                    handle.MarkFaulted(
                        new OperationCanceledException($"Coroutine (Id: {id}) was stopped via the Stop method."));
            }
            else
            {
                if (handle.IsCompleted)
                    throw new InvalidOperationException(
                        $"Coroutine (Id: {id}) has already completed and cannot be stopped.");

                throw new ArgumentException($"Coroutine (Id: {id}) could not be found or is not currently running.",
                    nameof(handle));
            }
        }

        private IEnumerator WrapRoutine(
            IEnumerator routine,
            AsyncProcessHandle handle,
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
            public RunningCoroutineInfo(Coroutine unityCoroutine, AsyncProcessHandle handle)
            {
                UnityCoroutine = unityCoroutine;
                Handle = handle;
            }

            public Coroutine UnityCoroutine { get; }
            public AsyncProcessHandle Handle { get; }
        }
    }
}