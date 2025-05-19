using System;
using NUnit.Framework;
using UnityScreenNavigator.Runtime.Foundation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation.CoroutineSystem
{
    internal sealed class AsyncStatusYieldHandlerTest
    {
        private AsyncStatusYieldHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _handler = new AsyncStatusYieldHandler();
        }

        [Test]
        public void TryHandle_Yieldされた値がAsyncStatusでない場合_Falseを返すこと()
        {
            // Arrange
            var nonAsyncStatusValue = new object();

            // Act
            var handled = _handler.TryHandle(nonAsyncStatusValue, out var result);

            // Assert
            Assert.That(handled, Is.False);
        }

        [Test]
        public void TryHandle_YieldされたAsyncStatusが未完了の場合_Trueを返しPauseForNextTickを返すこと()
        {
            // Arrange
            var awaitedStatus = new AsyncStatus(); // 未完了

            // Act
            var handled = _handler.TryHandle(awaitedStatus, out var result);

            // Assert
            Assert.That(handled, Is.True);
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.PauseForNextTick));
        }

        [Test]
        public void TryHandle_YieldされたAsyncStatusが正常完了済みの場合_Trueを返しKeepRunningInCurrentTickを返すこと()
        {
            // Arrange
            var awaitedStatus = new AsyncStatus();
            awaitedStatus.MarkCompleted(); // 正常完了

            // Act
            var handled = _handler.TryHandle(awaitedStatus, out var result);

            // Assert
            Assert.That(handled, Is.True);
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.KeepRunningInCurrentTick));
        }

        [Test]
        public void TryHandle_YieldされたAsyncStatusが失敗済みの場合_Trueを返しFaultCoroutineと例外を返すこと()
        {
            // Arrange
            var awaitedStatus = new AsyncStatus();
            var exception = new InvalidOperationException("Awaited task failed");
            awaitedStatus.MarkFaulted(exception); // 失敗

            // Act
            var handled = _handler.TryHandle(awaitedStatus, out var result);

            // Assert
            Assert.That(handled, Is.True);
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.FaultCoroutine));
            Assert.That(result.ExceptionToReport, Is.SameAs(exception));
        }
    }
}