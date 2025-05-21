// namespace UnityScreenNavigator.Runtime.Foundation
// {
//     /// <summary>
//     /// <see cref="CoroutineScheduler"/> によって管理される進行中のコルーチン操作を表します。
//     /// 操作のステータスと一意の識別子へのアクセスを提供します。
//     /// </summary>
//     public interface ICoroutineOperation
//     {
//         /// <summary>
//         /// このコルーチン操作の一意の識別子。
//         /// </summary>
//         int Id { get; }
//
//         /// <summary>
//         /// このコルーチン操作の <see cref="AsyncStatus"/>。
//         /// コルーチン内でこのステータスを yield したり (例: <c>yield return operation.Status;</c>)、
//         /// そのプロパティ (<see cref="AsyncStatus.IsCompleted"/>, <see cref="AsyncStatus.IsFaulted"/>, <see cref="AsyncStatus.Exception"/>) を確認したりできます。
//         /// </summary>
//         AsyncStatus Status { get; }
//     }
// }

