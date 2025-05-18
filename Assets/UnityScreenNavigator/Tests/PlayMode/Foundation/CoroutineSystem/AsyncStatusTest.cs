using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityScreenNavigator.Runtime.Foundation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation.CoroutineSystem
{
    internal sealed class AsyncStatusTest
    {
        [Test]
        public void Constructor_初期状態_IsCompletedFalse_IsFaultedFalse_ExceptionNull_AllExceptionsEmpty()
        {
            // Arrange & Act
            var status = new AsyncStatus();

            // Assert
            Assert.That(status.IsCompleted, Is.False);
            Assert.That(status.IsFaulted, Is.False);
            Assert.That(status.Exception, Is.Null);
            Assert.That(status.AllExceptions, Is.Not.Null.And.Empty);
        }

        [Test]
        public void MarkCompleted_未完了の時_IsCompletedがTrueになること()
        {
            // Arrange
            var status = new AsyncStatus();

            // Act
            status.MarkCompleted();

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.False); // 念のため確認
        }

        [Test]
        public void MarkCompleted_既に完了している時_何も変わらないこと()
        {
            // Arrange
            var status = new AsyncStatus();
            status.MarkCompleted(); // 最初に完了させる

            // Act
            status.MarkCompleted(); // 再度呼び出し

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.False);
        }

        [Test]
        public void MarkFaulted_単一例外で未完了の時_IsCompletedTrue_IsFaultedTrue_Exception設定_AllExceptionsに設定()
        {
            // Arrange
            var status = new AsyncStatus();
            var exception = new InvalidOperationException("Test exception");

            // Act
            status.MarkFaulted(exception);

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.True);
            Assert.That(status.Exception, Is.SameAs(exception));
            Assert.That(status.AllExceptions, Is.Not.Null);
            Assert.That(status.AllExceptions.Count, Is.EqualTo(1));
            Assert.That(status.AllExceptions[0], Is.SameAs(exception));
        }

        [Test]
        public void MarkFaulted_単一例外で既に完了している時_何も変わらないこと()
        {
            // Arrange
            var status = new AsyncStatus();
            var initialException = new ArgumentNullException("Initial");
            status.MarkFaulted(initialException); // 最初に失敗させる

            var subsequentException = new InvalidOperationException("Subsequent");

            // Act
            status.MarkFaulted(subsequentException); // 完了後に再度呼び出し

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.True);
            Assert.That(status.Exception, Is.SameAs(initialException), "最初の例外が保持されるべき");
            Assert.That(status.AllExceptions.Count, Is.EqualTo(1), "例外リストの数は変わらないべき");
            Assert.That(status.AllExceptions[0], Is.SameAs(initialException));
        }

        [Test]
        public void MarkFaulted_単一例外で正常完了している時_何も変わらないこと()
        {
            // Arrange
            var status = new AsyncStatus();
            status.MarkCompleted(); // 最初に正常完了させる

            var subsequentException = new InvalidOperationException("Subsequent");

            // Act
            status.MarkFaulted(subsequentException); // 完了後にFaultedを呼び出し

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.False, "正常完了後はFaultedにならない");
            Assert.That(status.Exception, Is.Null);
            Assert.That(status.AllExceptions, Is.Empty);
        }

        [Test]
        public void MarkFaulted_複数例外リストで未完了の時_IsCompletedTrue_IsFaultedTrue_Exceptionは最初の例外_AllExceptionsに全設定()
        {
            // Arrange
            var status = new AsyncStatus();
            var ex1 = new InvalidOperationException("Ex1");
            var ex2 = new ArgumentException("Ex2");
            var exceptions = new List<Exception> { ex1, ex2 };

            // Act
            status.MarkFaulted(exceptions);

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.True);
            Assert.That(status.Exception, Is.SameAs(ex1), "最初の例外がExceptionプロパティに設定される");
            Assert.That(status.AllExceptions, Is.Not.Null);
            Assert.That(status.AllExceptions.Count, Is.EqualTo(2));
            Assert.That(status.AllExceptions.Contains(ex1), Is.True);
            Assert.That(status.AllExceptions.Contains(ex2), Is.True);
        }

        [Test]
        public void MarkFaulted_空の複数例外リストで未完了の時_IsCompletedTrue_IsFaultedFalse_ExceptionNull()
        {
            // Arrange
            var status = new AsyncStatus();
            var exceptions = new List<Exception>(); // 空のリスト

            // Act
            status.MarkFaulted(exceptions);

            // Assert
            Assert.That(status.IsCompleted, Is.True, "IsCompleted は true になる");
            Assert.That(status.IsFaulted, Is.False, "空の例外リストでは IsFaulted は false");
            Assert.That(status.Exception, Is.Null, "Exception は null");
            Assert.That(status.AllExceptions, Is.Empty, "AllExceptions は空");
        }


        [Test]
        public void MarkFaulted_複数例外リストで既に完了している時_何も変わらないこと()
        {
            // Arrange
            var status = new AsyncStatus();
            var initialException = new ApplicationException("Initial");
            status.MarkFaulted(initialException); // 最初に失敗させる

            var ex1 = new InvalidOperationException("Ex1");
            var ex2 = new ArgumentException("Ex2");
            var subsequentExceptions = new List<Exception> { ex1, ex2 };

            // Act
            status.MarkFaulted(subsequentExceptions); // 完了後に再度呼び出し

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.True);
            Assert.That(status.Exception, Is.SameAs(initialException));
            Assert.That(status.AllExceptions.Count, Is.EqualTo(1));
            Assert.That(status.AllExceptions[0], Is.SameAs(initialException));
        }

        [Test]
        public void MarkFaulted_複数例外リストで正常完了している時_何も変わらないこと()
        {
            // Arrange
            var status = new AsyncStatus();
            status.MarkCompleted(); // 最初に正常完了させる

            var ex1 = new InvalidOperationException("Ex1");
            var subsequentExceptions = new List<Exception> { ex1 };

            // Act
            status.MarkFaulted(subsequentExceptions);

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.False);
            Assert.That(status.Exception, Is.Null);
            Assert.That(status.AllExceptions, Is.Empty);
        }


        [Test]
        public void IsFaulted_例外がない場合_Falseを返すこと()
        {
            // Arrange
            var status = new AsyncStatus();

            // Act & Assert
            Assert.That(status.IsFaulted, Is.False);
            status.MarkCompleted(); // 正常完了しても Faulted ではない
            Assert.That(status.IsFaulted, Is.False);
        }

        [Test]
        public void IsFaulted_例外がある場合_Trueを返すこと()
        {
            // Arrange
            var status = new AsyncStatus();
            status.MarkFaulted(new Exception("Test"));

            // Act & Assert
            Assert.That(status.IsFaulted, Is.True);
        }

        [Test]
        public void Wait_未完了の時_IsCompletedがFalseの間nullを返し続けること()
        {
            // Arrange
            var status = new AsyncStatus();
            var waiter = status.Wait();
            var stepCount = 0;

            // Act & Assert
            // 最初の数ステップは IsCompleted が false なので null を返すはず
            Assert.That(waiter.MoveNext(), Is.True, "MoveNext 1 should be true");
            Assert.That(waiter.Current, Is.Null, "Current 1 should be null");
            stepCount++;

            Assert.That(waiter.MoveNext(), Is.True, "MoveNext 2 should be true");
            Assert.That(waiter.Current, Is.Null, "Current 2 should be null");
            stepCount++;

            // 途中で完了させる
            status.MarkCompleted();

            Assert.That(waiter.MoveNext(), Is.False, "MoveNext after completion should be false");
            Assert.That(stepCount, Is.EqualTo(2), "Specified steps should have occurred before completion");
        }

        [Test]
        public void Wait_最初から完了済みの時_すぐに終了すること()
        {
            // Arrange
            var status = new AsyncStatus();
            status.MarkCompleted();
            var waiter = status.Wait();

            // Act & Assert
            Assert.That(waiter.MoveNext(), Is.False); // IsCompleted なのですぐに false
        }

        [Test]
        public void Wait_最初から失敗済みの時_すぐに終了すること()
        {
            // Arrange
            var status = new AsyncStatus();
            status.MarkFaulted(new Exception("fail")); // IsCompleted も true になる
            var waiter = status.Wait();

            // Act & Assert
            Assert.That(waiter.MoveNext(), Is.False);
        }

        // Createメソッドのテストは非同期処理の性質上、少し複雑になります。
        // MonitorAsyncがasync voidであるため、その完了を直接待つのが難しいです。
        // テストでは、Taskが完了するか例外をスローするのを待ってからStatusを検証します。
        // async voidメソッドのテストは通常、同期コンテキストのカスタマイズや、
        // 副作用（この場合はStatusの更新）をポーリングすることで行われます。

        [Test]
        public async Task Create_Taskが正常に完了する場合_Statusが完了になること()
        {
            // Arrange
            var tcs = new TaskCompletionSource<bool>();
            Func<Task> asyncFunc = () => tcs.Task;

            // Act
            var status = AsyncStatus.Create(asyncFunc);

            // Assert
            Assert.That(status.IsCompleted, Is.False, "Task開始直後は未完了");

            tcs.SetResult(true); // Taskを完了させる

            // async void の MonitorAsync が完了するのを少し待つ
            // より堅牢なテストのためには、ポーリングやカスタム同期コンテキストが必要になる場合があります。
            // ここでは簡略化のため、Task.Delayを使用します。
            await Task.Delay(100); // MonitorAsync内のawaitとMarkCompletedが実行されるのに十分な時間

            Assert.That(status.IsCompleted, Is.True, "Task完了後、Statusは完了になるべき");
            Assert.That(status.IsFaulted, Is.False);
        }

        [Test]
        public async Task Create_Taskが例外をスローする場合_Statusが失敗になること()
        {
            // Arrange
            var tcs = new TaskCompletionSource<bool>();
            var testException = new InvalidOperationException("Async task failed");
            Func<Task> asyncFunc = () =>
            {
                tcs.SetException(testException);
                return tcs.Task;
            };
            // 代わりに Task.FromException を使うこともできます
            // Func<Task> asyncFunc = () => Task.FromException(testException);


            // Act
            var status = AsyncStatus.Create(asyncFunc);

            // Assert
            Assert.That(status.IsCompleted, Is.False, "Task開始直後は未完了");

            // Taskが例外で完了するのを待つ (Create内では捕捉される)
            Exception? caughtInTest = null;
            try
            {
                await asyncFunc(); // これ自体は例外を再スローする
            }
            catch (Exception ex)
            {
                caughtInTest = ex; // Createに渡したasyncFuncがスローした例外をキャッチ
            }

            Assert.That(caughtInTest, Is.SameAs(testException), "asyncFuncは指定した例外をスローするべき");


            // async void の MonitorAsync が完了するのを少し待つ
            await Task.Delay(100);

            Assert.That(status.IsCompleted, Is.True, "Task失敗後、Statusは完了になるべき");
            Assert.That(status.IsFaulted, Is.True, "Task失敗後、StatusはFaultedになるべき");
            Assert.That(status.Exception, Is.SameAs(testException));
            Assert.That(status.AllExceptions.Contains(testException), Is.True);
        }

        [Test]
        public async Task Create_Taskが既に完了している場合_Statusが即座に完了に近い状態になること()
        {
            // Arrange
            Func<Task> asyncFunc = () => Task.CompletedTask;

            // Act
            var status = AsyncStatus.Create(asyncFunc);

            // async void の MonitorAsync が完了するのを少し待つ
            await Task.Delay(50); // 通常は非常に速いはず

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.False);
        }

        [Test]
        public async Task Create_Taskが既に失敗している場合_Statusが即座に失敗に近い状態になること()
        {
            // Arrange
            var testException = new OperationCanceledException("Cancelled");
            Func<Task> asyncFunc = () => Task.FromException(testException);

            // Act
            var status = AsyncStatus.Create(asyncFunc);

            // async void の MonitorAsync が完了するのを少し待つ
            await Task.Delay(50);

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.True);
            Assert.That(status.Exception, Is.SameAs(testException));
        }
    }
}