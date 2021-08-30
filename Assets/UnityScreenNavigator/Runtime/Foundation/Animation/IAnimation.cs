namespace UnityScreenNavigator.Runtime.Foundation.Animation
{
    public interface IAnimation
    {
        float Duration { get; }

        void SetTime(float time);
    }
}