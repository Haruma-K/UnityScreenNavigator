using System;
using System.Collections;
using NUnit.Framework;
using UnityScreenNavigator.Runtime.Foundation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation
{
    internal sealed class CoroutineHandleTest
    {
        [Test]
        public void Constructor_初期化後_StatusがNotNullかつ未完了であること()
        {
            // Arrange
            var simpleRoutine = SimpleRoutine();

            // Act
            var handle = new CoroutineHandle(simpleRoutine);

            // Assert
            Assert.That(handle.Status, Is.Not.Null);
            Assert.That(handle.Status.IsCompleted, Is.False);
            Assert.That(handle.Status.IsFaulted, Is.False);
            Assert.That(handle.Status.Exception, Is.Null);
        }

        [Test]
        public void Step_コルーチンが初回MoveNextで完了する場合_Completedを返しStatusが完了になること()
        {
            // Arrange
            // MoveNext() が false を返す IEnumerator (ステップがないため)
            var routine = SimpleRoutine();
            var handle = new CoroutineHandle(routine);

            // Act
            var result = handle.Step();

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Completed));
            Assert.That(handle.Status.IsCompleted, Is.True);
            Assert.That(handle.Status.IsFaulted, Is.False);
            Assert.That(handle.Status.Exception, Is.Null);
        }

        [Test]
        public void Step_コルーチンがnullをyieldする場合_Continueを返しStatusが未完了であること()
        {
            // Arrange
            var routine = SimpleRoutine(() => null);
            var handle = new CoroutineHandle(routine);

            // Act
            var result = handle.Step();

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Continue));
            Assert.That(handle.Status.IsCompleted, Is.False);
            Assert.That(handle.Status.IsFaulted, Is.False);
        }

        [Test]
        public void Step_コルーチンが未完了のAsyncStatusをyieldする場合_Continueを返しStatusが未完了であること()
        {
            // Arrange
            var awaitedStatus = new AsyncStatus();
            var routine = SimpleRoutine(() => awaitedStatus);
            var handle = new CoroutineHandle(routine);

            // Act
            var result = handle.Step();

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Continue));
            Assert.That(handle.Status.IsCompleted, Is.False);
            Assert.That(handle.Status.IsFaulted, Is.False);
        }

        [Test]
        public void Step_コルーチンが正常完了済みのAsyncStatusをyieldし次に完了する場合_Completedを返すこと()
        {
            // Arrange
            var awaitedStatus = new AsyncStatus();
            awaitedStatus.MarkCompleted(); // 事前に完了させておく
            // 最初に awaitedStatus を yield し、その直後にコルーチンが完了する (MoveNext が false を返す)
            var finalRoutine = SimpleRoutine(() => awaitedStatus);
            var handle = new CoroutineHandle(finalRoutine);

            // Act
            // 1回目のStepでawaitedStatusを処理し、コルーチン内のwhile(true)ループにより
            // 即座に次の _routine.MoveNext() が試行され、それが false を返すため Completed になる。
            var result = handle.Step();


            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Completed), "完了したAsyncStatusを処理し、コルーチンも即座に完了する");
            Assert.That(handle.Status.IsCompleted, Is.True);
            Assert.That(handle.Status.IsFaulted, Is.False);
        }

        [Test]
        public void Step_コルーチンが正常完了済みのAsyncStatusをyieldし次にnullをyieldする場合_Continueを返すこと()
        {
            // Arrange
            var awaitedStatus = new AsyncStatus();
            awaitedStatus.MarkCompleted();

            // awaitedStatus を yield した後に、さらに null を yield するコルーチン
            var routine = SimpleRoutine(() => awaitedStatus, () => null);
            var handle = new CoroutineHandle(routine);

            // Act
            // 最初のStepで awaitedStatus を処理し、内部ループで次の yield return null まで進み、そこで停止する
            var result = handle.Step();

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Continue));
            Assert.That(handle.Status.IsCompleted, Is.False);
            Assert.That(handle.Status.IsFaulted, Is.False);
        }


        [Test]
        public void Step_コルーチンが失敗済みのAsyncStatusをyieldする場合_Faultedを返しStatusが失敗になること()
        {
            // Arrange
            var awaitedStatus = new AsyncStatus();
            var awaitedException = new InvalidOperationException("Awaited task failed");
            awaitedStatus.MarkFaulted(awaitedException);

            var routine = SimpleRoutine(() => awaitedStatus);
            var handle = new CoroutineHandle(routine);

            // Act
            var result = handle.Step();

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Faulted));
            Assert.That(handle.Status.IsCompleted, Is.True);
            Assert.That(handle.Status.IsFaulted, Is.True);
            Assert.That(handle.Status.Exception, Is.EqualTo(awaitedException));
        }

        [Test]
        public void Step_コルーチン実行中に例外が発生する場合_Faultedを返しStatusが失敗になること()
        {
            // Arrange
            var exceptionToThrow = new ArithmeticException("Coroutine calculation error");
            var routine = SimpleRoutine(() => throw exceptionToThrow);
            var handle = new CoroutineHandle(routine);

            // Act
            // Step内で例外をキャッチし、適切に処理することをテスト
            var result = CoroutineStepResult.Continue; // 初期値
            Exception caughtInTest = null;
            try
            {
                // CoroutineHandle.Step() は MoveNext() を呼び出し、そこで例外が発生する。
                // Step() メソッド自体が例外をスローしないことを確認。
                result = handle.Step();
            }
            catch (Exception ex)
            {
                // このブロックが実行された場合、CoroutineHandle.Step() が例外を外にスローしたことになる。
                // これは期待される動作ではない。
                caughtInTest = ex;
            }

            // Assert
            Assert.That(caughtInTest, Is.Null, "CoroutineHandle.Step() は例外を外部にスローすべきではない");
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Faulted));
            Assert.That(handle.Status.IsCompleted, Is.True);
            Assert.That(handle.Status.IsFaulted, Is.True);
            Assert.That(handle.Status.Exception, Is.TypeOf<ArithmeticException>());
            Assert.That(handle.Status.Exception, Is.SameAs(exceptionToThrow));
        }

        [Test]
        public void Step_既に完了しているコルーチンに対してStepを呼ぶ場合_Completedを返すこと()
        {
            // Arrange
            var routine = SimpleRoutine(); // すぐ完了するコルーチン
            var handle = new CoroutineHandle(routine);
            handle.Step(); // これで完了する

            // Act
            var result = handle.Step(); // 再度Stepを呼ぶ

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Completed));
            Assert.That(handle.Status.IsCompleted, Is.True); // 状態は維持
        }

        [Test]
        public void Step_既に失敗しているコルーチンに対してStepを呼ぶ場合_Faultedを返すこと()
        {
            // Arrange
            var exceptionToThrow = new AccessViolationException("Failed");
            var routine = SimpleRoutine(() => throw exceptionToThrow);
            var handle = new CoroutineHandle(routine);
            handle.Step();

            // Act
            var result = handle.Step(); // 再度Stepを呼ぶ

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Faulted));
            Assert.That(handle.Status.IsFaulted, Is.True);
            Assert.That(handle.Status.IsCompleted, Is.True);
        }


        [Test]
        public void Step_コルーチンがAsyncStatus以外のオブジェクトをyieldする場合_Continueを返すこと()
        {
            // Arrange
            var routine = SimpleRoutine(() => 123); // int を yield する
            var handle = new CoroutineHandle(routine);

            // Act
            var result = handle.Step();

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Continue));
            Assert.That(handle.Status.IsCompleted, Is.False);
            Assert.That(handle.Status.IsFaulted, Is.False);
        }

        [Test]
        public void Step_複数ステップ後にコルーチンが完了する場合_正しくCompletedになること()
        {
            // Arrange
            var routine = SimpleRoutine(() => null, () => null, () => "step 3 then complete"); // 3回 yield して完了
            var handle = new CoroutineHandle(routine);

            // Act & Assert
            // 1回目のStep
            var result1 = handle.Step();
            Assert.That(result1, Is.EqualTo(CoroutineStepResult.Continue), "Step 1 should continue");
            Assert.That(handle.Status.IsCompleted, Is.False, "Status after step 1 should not be completed");

            // 2回目のStep
            var result2 = handle.Step();
            Assert.That(result2, Is.EqualTo(CoroutineStepResult.Continue), "Step 2 should continue");
            Assert.That(handle.Status.IsCompleted, Is.False, "Status after step 2 should not be completed");

            // 3回目のStep (ここで最後の yield が行われる)
            var result3 = handle.Step();
            Assert.That(result3,
                Is.EqualTo(CoroutineStepResult.Continue),
                "Step 3 should continue as it yields 'step 3 then complete'");
            Assert.That(handle.Status.IsCompleted, Is.False, "Status after step 3 should not be completed");

            // 4回目のStep (ここで MoveNext() が false を返し、コルーチンが完了する)
            var result4 = handle.Step();
            Assert.That(result4, Is.EqualTo(CoroutineStepResult.Completed), "Step 4 should complete the coroutine");
            Assert.That(handle.Status.IsCompleted, Is.True, "Status after step 4 should be completed");
            Assert.That(handle.Status.IsFaulted, Is.False);
        }

        [Test]
        public void Step_完了済みAsyncStatusをyield後次のステップがnullの場合_Continueを返すこと()
        {
            // Arrange
            var completedAsyncStatus = new AsyncStatus();
            completedAsyncStatus.MarkCompleted();

            var routine = SimpleRoutine(() => completedAsyncStatus, () => null);
            var handle = new CoroutineHandle(routine);

            // Act
            // 最初のStep()呼び出し:
            // 1. _routine.MoveNext() -> true, _routine.Current is completedAsyncStatus
            // 2. awaited.IsCompleted is true, awaited.IsFaulted is false
            // 3. ループが継続し、再度 _routine.MoveNext() -> true, _routine.Current is null
            // 4. _routine.Current is not AsyncStatus -> CoroutineStepResult.Continue が返る
            var result = handle.Step();

            // Assert
            Assert.That(result, Is.EqualTo(CoroutineStepResult.Continue));
            Assert.That(handle.Status.IsCompleted, Is.False);
        }

        // Helper method to create a simple IEnumerator that yields specified values
        private static IEnumerator SimpleRoutine(params Func<object>[] steps)
        {
            foreach (var step in steps)
                yield return step.Invoke();
        }
    }
}