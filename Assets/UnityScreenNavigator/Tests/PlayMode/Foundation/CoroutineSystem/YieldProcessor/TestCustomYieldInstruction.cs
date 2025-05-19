using UnityEngine;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation.CoroutineSystem
{
    public class TestCustomYieldInstruction : CustomYieldInstruction
    {
        private readonly bool _initiallyKeepsWaiting;
        private int _keepWaitingCount;

        /// <summary>
        ///     keepWaitingがtrueを返す回数を指定します。
        ///     0を指定すると、最初からfalseを返します。
        /// </summary>
        public TestCustomYieldInstruction(int keepWaitingForNTimes = 0)
        {
            _keepWaitingCount = keepWaitingForNTimes;
        }

        public bool WasQueried => QueryCount > 0;
        public int QueryCount { get; private set; }

        public override bool keepWaiting
        {
            get
            {
                QueryCount++;
                if (_keepWaitingCount > 0)
                {
                    _keepWaitingCount--;
                    return true;
                }

                return false;
            }
        }
    }
}