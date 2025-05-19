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
        public void Constructor_初期状態()
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
        public void MarkCompleted_IsCompletedがTrueになること()
        {
            // Arrange
            var status = new AsyncStatus();

            // Act
            status.MarkCompleted();

            // Assert
            Assert.That(status.IsCompleted, Is.True);
        }

        [Test]
        public void MarkFaulted_単一例外()
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
        public void MarkFaulted_複数例外リスト()
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
        public async Task Create_Taskが正常に完了する場合_Statusが完了になること()
        {
            // Arrange
            var tcs = new TaskCompletionSource<bool>();

            // Act
            var status = AsyncStatus.Create(() => tcs.Task);

            // Assert
            Assert.That(status.IsCompleted, Is.False, "Task開始直後は未完了");

            tcs.SetResult(true); // Taskを完了させる

            await Task.Delay(100); // 少し待つ

            Assert.That(status.IsCompleted, Is.True, "Task完了後、Statusは完了になるべき");
            Assert.That(status.IsFaulted, Is.False);
        }

        [Test]
        public async Task Create_Taskが例外をスローする場合_Statusが失敗になること()
        {
            // Arrange
            var testException = new InvalidOperationException("Async task failed");

            // Act
            var status = AsyncStatus.Create(() => throw testException);
            await Task.Delay(100); // 少し待つ

            // Assert
            Assert.That(status.IsCompleted, Is.True, "Task失敗後、Statusは完了になるべき");
            Assert.That(status.IsFaulted, Is.True, "Task失敗後、StatusはFaultedになるべき");
            Assert.That(status.Exception, Is.SameAs(testException));
            Assert.That(status.AllExceptions.Contains(testException), Is.True);
        }
    }
}