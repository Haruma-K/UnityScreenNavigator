using NUnit.Framework;
using UnityScreenNavigator.Runtime.Foundation.Animation;

namespace UnityScreenNavigator.Tests.PlayMode.Foundation
{
    public class AnimationPlayerTest
    {
        [Test]
        public void Play_Progressing()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            player.Play();
            Assert.That(animation.Progress, Is.EqualTo(0.0f));
            player.Update(0.1f);
            Assert.That(animation.Progress, Is.EqualTo(0.1f / 1.0f));
        }
        
        [Test]
        public void NotPlay_NotProgressing()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            Assert.That(animation.Progress, Is.EqualTo(0.0f));
            player.Update(0.1f);
            Assert.That(animation.Progress, Is.EqualTo(0.0f));
        }
        
        [Test]
        public void Stop_NotProgressing()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            player.Play();
            Assert.That(animation.Progress, Is.EqualTo(0.0f));
            player.Update(0.1f);
            Assert.That(animation.Progress, Is.EqualTo(0.1f / 1.0f));
            player.Stop();
            player.Update(0.1f);
            Assert.That(animation.Progress, Is.EqualTo(0.1f / 1.0f));
        }
        
        [Test]
        public void Reset_CanReset()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            player.Play();
            Assert.That(animation.Progress, Is.EqualTo(0.0f));
            player.Update(0.1f);
            Assert.That(animation.Progress, Is.EqualTo(0.1f / 1.0f));
            player.Reset();
            Assert.That(animation.Progress, Is.EqualTo(0.0f));
        }
        
        [Test]
        public void SetTime_CanSet()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            player.SetTime(0.3f);
            Assert.That(animation.Progress, Is.EqualTo(0.3f));
        }
        
        [Test]
        public void SetTime_NegativeValue_ProgressIsZero()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            player.SetTime(-1f);
            Assert.That(animation.Progress, Is.EqualTo(0.0f));
        }
        
        [Test]
        public void SetTime_LargeValue_ProgressIsOne()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            player.SetTime(float.MaxValue);
            Assert.That(animation.Progress, Is.EqualTo(1.0f));
        }
        
        [Test]
        public void IsFinished_SetTimeBeyondDuration_True()
        {
            const float duration = 1.0f;
            var animation = new FakeAnimation(duration);
            var player = new AnimationPlayer(animation);
            player.SetTime(float.MaxValue);
            Assert.That(player.IsFinished, Is.True);
        }
    }
}
