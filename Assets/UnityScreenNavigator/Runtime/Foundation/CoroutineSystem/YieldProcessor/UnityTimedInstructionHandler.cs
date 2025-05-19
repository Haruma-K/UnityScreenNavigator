using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal class UnityTimedInstructionHandler : IYieldInstructionHandler
    {
        private static readonly FieldInfo SecondsFieldInfoForWaitForSeconds;

        private readonly Dictionary<object, float> _instructionEndTimes = new();

        static UnityTimedInstructionHandler()
        {
            SecondsFieldInfoForWaitForSeconds =
                typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.Instance | BindingFlags.NonPublic);
            if (SecondsFieldInfoForWaitForSeconds == null)
                Debug.LogWarning("UnityScreenNavigator.Runtime.Foundation.UnityTimedInstructionHandler: " +
                                 "UnityEngine.WaitForSeconds の内部フィールド 'm_Seconds' が見つかりませんでした。" +
                                 "WaitForSeconds の処理が正しく機能しない可能性があります。");
        }

        public bool TryHandle(object yieldInstruction, out YieldInstructionResult result)
        {
            if (yieldInstruction is WaitForSeconds wfs)
            {
                if (SecondsFieldInfoForWaitForSeconds == null)
                {
                    // フィールド情報がない場合は、このハンドラでは WaitForSeconds を安全に処理できない
                    Debug.LogWarning("UnityTimedInstructionHandler: 'm_Seconds' フィールド情報がないため、WaitForSeconds を処理できません。");
                    result = YieldInstructionResult.PauseForNextTick();
                    return true;
                }

                if (!_instructionEndTimes.TryGetValue(wfs, out var endTime))
                {
                    var duration = (float)SecondsFieldInfoForWaitForSeconds.GetValue(wfs);
                    endTime = Time.time + duration;
                    _instructionEndTimes[wfs] = endTime;

                    if (Time.time >= endTime) // duration <= 0 の場合も処理
                    {
                        result = YieldInstructionResult.KeepRunningInCurrentTick();
                        _instructionEndTimes.Remove(wfs);
                    }
                    else
                    {
                        result = YieldInstructionResult.PauseForNextTick();
                    }
                }
                else
                {
                    if (Time.time >= endTime)
                    {
                        result = YieldInstructionResult.KeepRunningInCurrentTick();
                        _instructionEndTimes.Remove(wfs);
                    }
                    else
                    {
                        result = YieldInstructionResult.PauseForNextTick();
                    }
                }

                return true;
            }

            if (yieldInstruction is WaitForSecondsRealtime wfsr) // ★WaitForSecondsRealtime の処理を追加
            {
                if (!_instructionEndTimes.TryGetValue(wfsr, out var endTime))
                {
                    // WaitForSecondsRealtime には public の waitTime プロパティがあるため、リフレクション不要
                    var duration = wfsr.waitTime;
                    endTime = Time.realtimeSinceStartup + duration;
                    _instructionEndTimes[wfsr] = endTime;

                    if (Time.realtimeSinceStartup >= endTime) // duration <= 0 の場合も処理
                    {
                        result = YieldInstructionResult.KeepRunningInCurrentTick();
                        _instructionEndTimes.Remove(wfsr);
                    }
                    else
                    {
                        result = YieldInstructionResult.PauseForNextTick();
                    }
                }
                else
                {
                    if (Time.realtimeSinceStartup >= endTime)
                    {
                        result = YieldInstructionResult.KeepRunningInCurrentTick();
                        _instructionEndTimes.Remove(wfsr);
                    }
                    else
                    {
                        result = YieldInstructionResult.PauseForNextTick();
                    }
                }

                return true;
            }

            result = default;
            return false;
        }

        public void CleanupInstructionState(object yieldInstruction)
        {
            // yieldInstruction が WaitForSeconds または WaitForSecondsRealtime のインスタンスであれば、
            // 辞書から削除を試みる。キーが存在しなければ Remove は何もしない。
            _instructionEndTimes.Remove(yieldInstruction);
        }
    }
}