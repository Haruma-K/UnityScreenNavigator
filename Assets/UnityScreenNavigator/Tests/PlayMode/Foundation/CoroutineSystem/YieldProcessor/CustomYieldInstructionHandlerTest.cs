using NUnit.Framework;
using UnityScreenNavigator.Runtime.Foundation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation.CoroutineSystem
{
    internal sealed class CustomYieldInstructionHandlerTest
    {
        private CustomYieldInstructionHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _handler = new CustomYieldInstructionHandler();
        }

        [Test]
        public void TryHandle_Yieldされた値がCustomYieldInstructionでない場合_Falseを返すこと()
        {
            // Arrange
            var nonCustomYieldValue = new object();

            // Act
            var handled = _handler.TryHandle(nonCustomYieldValue, out var result);

            // Assert
            Assert.That(handled, Is.False);
        }

        [Test]
        public void TryHandle_CustomYieldInstructionのKeepWaitingがTrueの場合_Trueを返しPauseForNextTickを返すこと()
        {
            // Arrange
            var customInstruction = new TestCustomYieldInstruction(1);

            // Act
            var handled = _handler.TryHandle(customInstruction, out var result);

            // Assert
            Assert.That(handled, Is.True);
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.PauseForNextTick));
            Assert.That(customInstruction.QueryCount, Is.EqualTo(1));
        }

        [Test]
        public void TryHandle_CustomYieldInstructionのKeepWaitingがFalseの場合_Trueを返しKeepRunningInCurrentTickを返すこと()
        {
            // Arrange
            var customInstruction = new TestCustomYieldInstruction();

            // Act
            var handled = _handler.TryHandle(customInstruction, out var result);

            // Assert
            Assert.That(handled, Is.True);
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.KeepRunningInCurrentTick));
            Assert.That(customInstruction.QueryCount, Is.EqualTo(1));
        }
    }
}