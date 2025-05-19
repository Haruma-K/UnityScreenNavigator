using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityScreenNavigator.Runtime.Foundation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation.CoroutineSystem
{
    internal sealed class YieldProcessorTest
    {
        private List<IYieldInstructionHandler> _handlers;
        private Mock<IYieldInstructionHandler> _mockHandler1;
        private Mock<IYieldInstructionHandler> _mockHandler2;
        private YieldProcessor _processor;

        [SetUp]
        public void SetUp()
        {
            _handlers = new List<IYieldInstructionHandler>();
            _mockHandler1 = new Mock<IYieldInstructionHandler>();
            _mockHandler2 = new Mock<IYieldInstructionHandler>();

            _mockHandler1.Setup(h => h.TryHandle(It.IsAny<object>(), out It.Ref<YieldInstructionResult>.IsAny))
                .Returns(false);
            _mockHandler2.Setup(h => h.TryHandle(It.IsAny<object>(), out It.Ref<YieldInstructionResult>.IsAny))
                .Returns(false);
        }

        [Test]
        public void Process_ハンドラ未登録の場合_デフォルトのPauseForNextTick結果を返すこと()
        {
            // Arrange
            _processor = new YieldProcessor(new List<IYieldInstructionHandler>());
            var yieldValue = new object();

            // Act
            var result = _processor.Process(yieldValue);

            // Assert
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.PauseForNextTick));
        }

        [Test]
        public void Process_処理できるハンドラがない場合_デフォルトのPauseForNextTick結果を返すこと()
        {
            // Arrange
            _handlers.Add(_mockHandler1.Object);
            _handlers.Add(_mockHandler2.Object);
            _processor = new YieldProcessor(_handlers);
            var yieldValue = new object();

            // Act
            var result = _processor.Process(yieldValue);

            // Assert
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.PauseForNextTick));
            _mockHandler1.Verify(h => h.TryHandle(yieldValue, out It.Ref<YieldInstructionResult>.IsAny), Times.Once);
            _mockHandler2.Verify(h => h.TryHandle(yieldValue, out It.Ref<YieldInstructionResult>.IsAny), Times.Once);
        }

        [Test]
        public void Process_最初のハンドラが処理する場合_その結果を返し後続ハンドラは呼ばないこと()
        {
            // Arrange
            var yieldValue = new AsyncStatus();
            var expectedResultStruct = YieldInstructionResult.KeepRunningInCurrentTick(); // モック設定用にstructのインスタンスが必要

            _mockHandler1.Setup(h => h.TryHandle(yieldValue, out expectedResultStruct))
                .Returns(true);

            _handlers.Add(_mockHandler1.Object);
            _handlers.Add(_mockHandler2.Object);
            _processor = new YieldProcessor(_handlers);

            // Act
            var actualResult = _processor.Process(yieldValue);

            // Assert
            Assert.That(actualResult.ActionType, Is.EqualTo(expectedResultStruct.ActionType));
            _mockHandler1.Verify(h => h.TryHandle(yieldValue, out It.Ref<YieldInstructionResult>.IsAny), Times.Once);
            _mockHandler2.Verify(h => h.TryHandle(It.IsAny<object>(), out It.Ref<YieldInstructionResult>.IsAny),
                Times.Never);
        }

        [Test]
        public void Process_2番目のハンドラが処理する場合_その結果を返すこと()
        {
            // Arrange
            var yieldValue = new TestCustomYieldInstruction();
            var expectedResultStruct = YieldInstructionResult.PauseForNextTick(); // モック設定用にstructのインスタンスが必要

            _mockHandler1.Setup(h => h.TryHandle(yieldValue, out It.Ref<YieldInstructionResult>.IsAny))
                .Returns(false);
            _mockHandler2.Setup(h => h.TryHandle(yieldValue, out expectedResultStruct))
                .Returns(true);

            _handlers.Add(_mockHandler1.Object);
            _handlers.Add(_mockHandler2.Object);
            _processor = new YieldProcessor(_handlers);

            // Act
            var actualResult = _processor.Process(yieldValue);

            // Assert
            Assert.That(actualResult.ActionType, Is.EqualTo(expectedResultStruct.ActionType));
            _mockHandler1.Verify(h => h.TryHandle(yieldValue, out It.Ref<YieldInstructionResult>.IsAny), Times.Once);
            _mockHandler2.Verify(h => h.TryHandle(yieldValue, out It.Ref<YieldInstructionResult>.IsAny), Times.Once);
        }

        [Test]
        public void Process_NullをYieldした場合_どのハンドラも処理しなければデフォルトのPauseForNextTickを返すこと()
        {
            // Arrange
            var realAsyncHandler = new AsyncStatusYieldHandler();
            var realCustomHandler = new CustomYieldInstructionHandler();
            _handlers.Add(realAsyncHandler);
            _handlers.Add(realCustomHandler);
            _processor = new YieldProcessor(_handlers);

            // Act
            var result = _processor.Process(null);

            // Assert
            Assert.That(result.ActionType, Is.EqualTo(YieldInstructionActionType.PauseForNextTick));
        }
    }
}