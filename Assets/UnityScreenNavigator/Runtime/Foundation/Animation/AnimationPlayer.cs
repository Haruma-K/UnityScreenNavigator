using System;

namespace UnityScreenNavigator.Runtime.Foundation.Animation
{
    internal class AnimationPlayer : IUpdatable
    {
        public AnimationPlayer(IAnimation animation)
        {
            Animation = animation;
            SetTime(0.0f);
        }

        public IAnimation Animation { get; }
        public bool IsPlaying { get; private set; }
        public float Time { get; private set; }
        public bool IsFinished => Time >= Animation.Duration;

        public void Update(float deltaTime)
        {
            if (!IsPlaying)
            {
                return;
            }

            SetTime(Time + deltaTime);
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void Reset()
        {
            SetTime(0.0f);
        }

        public void SetTime(float time)
        {
            time = Math.Max(0, Math.Min(Animation.Duration, time));
            Animation.SetTime(time);

            if (IsPlaying && time >= Animation.Duration)
            {
                Stop();
            }

            Time = time;
        }
    }
}