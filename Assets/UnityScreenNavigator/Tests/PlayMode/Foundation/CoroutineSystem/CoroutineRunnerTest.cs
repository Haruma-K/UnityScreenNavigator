using System;
using System.Collections;
using NUnit.Framework;
using UnityScreenNavigator.Runtime.Foundation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation.CoroutineSystem
{
    internal sealed class CoroutineRunnerTest
    {
        private static IEnumerator SimpleRoutine(int stepsToContinue, Action onComplete = null)
        {
            for (var i = 0; i < stepsToContinue; i++)
                yield return null;
            onComplete?.Invoke();
        }

        private static IEnumerator ThrowingRoutine(Exception ex, int stepsBeforeThrow = 0)
        {
            for (var i = 0; i < stepsBeforeThrow; i++)
                yield return null;
            throw ex;
        }

        private static IEnumerator YieldAsyncStatusRoutine(AsyncStatus statusToYield, bool thenComplete = false)
        {
            yield return statusToYield;
            if (thenComplete) yield break; // コルーチンを完了させる
            yield return null; // 完了しない場合はもう一度yield
        }

        [Test]
        public void Constructor_初期状態_内部キューは空であること()
        {
            // Arrange & Act
            var runner = new CoroutineRunner();
            // 内部キューはprivateなので直接は見れないが、Tickの挙動で確認する

            // Assert
            // Tickを呼んでも例外などが発生しないことを確認 (間接的なテスト)
            Assert.DoesNotThrow(() => runner.Tick());
        }

        [Test]
        public void Run_新しいコルーチンを登録_Statusを返し内部で処理が開始されること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var steps = 1;
            var routine = SimpleRoutine(steps);

            // Act
            var status = runner.Run(routine);

            // Assert
            Assert.That(status, Is.Not.Null);
            Assert.That(status.IsCompleted, Is.False, "実行直後は未完了のはず");

            // Tickを進めて完了するか確認
            runner.Tick(); // 1ステップ進める
            Assert.That(status.IsCompleted, Is.False, "1ステップだけではまだ完了しない");

            runner.Tick(); // 2ステップ目でSimpleRoutine(1)が完了する
            Assert.That(status.IsCompleted, Is.True, "指定ステップ後完了するはず");
        }

        [Test]
        public void Tick_実行中のコルーチンがない場合_何も起きないこと()
        {
            // Arrange
            var runner = new CoroutineRunner();

            // Act & Assert
            // 複数回Tickを呼んでも問題ないことを確認
            Assert.DoesNotThrow(() => runner.Tick());
            Assert.DoesNotThrow(() => runner.Tick());
        }

        [Test]
        public void Tick_単一の継続コルーチンがある場合_Stepが呼ばれ再度キューに追加されること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var routine = SimpleRoutine(2); // 2回nullを返す
            var status = runner.Run(routine);

            // Act
            runner.Tick(); // 1回目のMoveNext (yield return null)

            // Assert
            Assert.That(status.IsCompleted, Is.False, "1回目のTick後も継続中");

            // Act (2回目)
            runner.Tick(); // 2回目のMoveNext (yield return null)
            Assert.That(status.IsCompleted, Is.False, "2回目のTick後も継続中");

            // Act (3回目)
            runner.Tick(); // 3回目のMoveNext (コルーチン完了)
            Assert.That(status.IsCompleted, Is.True, "3回目のTick後完了");
        }

        [Test]
        public void Tick_単一の即完了コルーチンがある場合_Stepが呼ばれキューから削除されStatusが完了すること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var routine = SimpleRoutine(0); // すぐに完了する
            var status = runner.Run(routine);
            Assert.That(status.IsCompleted, Is.False, "Run直後は未完了");

            // Act
            runner.Tick();

            // Assert
            Assert.That(status.IsCompleted, Is.True);

            // もう一度Tickしても状態は変わらないはず（キューにいないので処理されない）
            var initialExceptionCount = status.AllExceptions.Count;
            runner.Tick();
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.False);
            Assert.That(status.AllExceptions.Count, Is.EqualTo(initialExceptionCount));
        }

        [Test]
        public void Tick_単一の失敗コルーチンがある場合_Stepが呼ばれキューから削除されStatusが失敗すること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var exception = new InvalidOperationException("test fail");
            var routine = ThrowingRoutine(exception); // すぐに例外
            var status = runner.Run(routine);
            Assert.That(status.IsCompleted, Is.False, "Run直後は未完了");
            Assert.That(status.IsFaulted, Is.False);


            // Act
            runner.Tick();

            // Assert
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.True);
            Assert.That(status.Exception, Is.SameAs(exception));

            // もう一度Tickしても状態は変わらないはず
            runner.Tick();
            Assert.That(status.IsCompleted, Is.True);
            Assert.That(status.IsFaulted, Is.True);
        }

        [Test]
        public void Tick_複数コルーチン_継続と完了が混在_各々正しく処理されること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var continueRoutine = SimpleRoutine(1); // 1回nullを返し、次に完了
            var completeRoutine = SimpleRoutine(0); // すぐに完了

            var statusContinue = runner.Run(continueRoutine);
            var statusComplete = runner.Run(completeRoutine);

            // Act
            runner.Tick();

            // Assert
            Assert.That(statusContinue.IsCompleted, Is.False, "継続コルーチンはまだ未完了");
            Assert.That(statusComplete.IsCompleted, Is.True, "即完了コルーチンは完了");

            // Act (2回目)
            runner.Tick();
            Assert.That(statusContinue.IsCompleted, Is.True, "継続コルーチンが2回目のTickで完了");
            Assert.That(statusComplete.IsCompleted, Is.True, "即完了コルーチンは完了のまま");
        }

        [Test]
        public void Tick_コルーチンが未完了AsyncStatusをyield_継続されること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var yieldedStatus = new AsyncStatus(); // 未完了のまま
            var routine = YieldAsyncStatusRoutine(yieldedStatus);
            var mainStatus = runner.Run(routine);

            // Act
            runner.Tick(); // 1回目のTick: yieldedStatus を yield し、継続

            // Assert
            Assert.That(mainStatus.IsCompleted, Is.False, "メインコルーチンは継続中");
            Assert.That(yieldedStatus.IsCompleted, Is.False, "yieldされたStatusも未完了");

            // yieldedStatus を完了させる
            yieldedStatus.MarkCompleted();
            runner.Tick(); // 2回目のTick: 完了したyieldedStatusを処理し、次のyield nullへ

            Assert.That(mainStatus.IsCompleted, Is.False, "メインコルーチンは次のステップへ進み、まだ継続中");

            runner.Tick(); // 3回目のTick: 次のyield nullを処理し、完了
            Assert.That(mainStatus.IsCompleted, Is.True, "メインコルーチンが完了");
        }

        [Test]
        public void Tick_コルーチンが完了済みAsyncStatusをyieldし自身も完了_完了すること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var yieldedStatus = new AsyncStatus();
            yieldedStatus.MarkCompleted(); // 事前に完了
            var routine = YieldAsyncStatusRoutine(yieldedStatus, true);
            var mainStatus = runner.Run(routine);

            // Act
            runner.Tick();

            // Assert
            Assert.That(mainStatus.IsCompleted, Is.True, "メインコルーチンは完了するはず");
            Assert.That(mainStatus.IsFaulted, Is.False);
        }

        [Test]
        public void Tick_コルーチンが失敗済みAsyncStatusをyield_失敗すること()
        {
            // Arrange
            var runner = new CoroutineRunner();
            var yieldedStatus = new AsyncStatus();
            var innerException = new TimeoutException("Inner task timed out");
            yieldedStatus.MarkFaulted(innerException); // 事前に失敗

            var routine = YieldAsyncStatusRoutine(yieldedStatus);
            var mainStatus = runner.Run(routine);

            // Act
            runner.Tick();

            // Assert
            Assert.That(mainStatus.IsCompleted, Is.True);
            Assert.That(mainStatus.IsFaulted, Is.True);
            Assert.That(mainStatus.Exception, Is.SameAs(innerException));
        }
    }
}