using System;
using System.Collections.Generic;

namespace UnityScreenNavigator.Runtime.Foundation
{
    internal static class AsyncStatusExtensions
    {
        /// <summary>
        ///     複数の AsyncStatus を同期的に結合します。
        ///     このメソッドを呼び出す時点で、全ての引数 statuses は完了済み (IsCompleted == true) である必要があります。
        ///     完了していないステータスがある場合は InvalidOperationException をスローします。
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     statuses のいずれかが完了していない (IsCompleted == false) 場合にスローされます。
        /// </exception>
        public static AsyncStatus Combine(params AsyncStatus[] statuses)
        {
            if (statuses == null || statuses.Length == 0)
                throw new ArgumentException(nameof(statuses));

            var combined = new AsyncStatus();
            var exceptions = new List<Exception>();

            foreach (var status in statuses)
            {
                if (!status.IsCompleted)
                    throw new InvalidOperationException(
                        "All AsyncStatus instances must be completed before calling Combine. " +
                        "Consider awaiting or ensuring completion of all statuses."
                    );

                if (status.IsFaulted)
                    exceptions.AddRange(status.AllExceptions);
            }

            if (exceptions.Count > 0)
                combined.MarkFaulted(exceptions);
            else
                combined.MarkCompleted();

            return combined;
        }
    }
}