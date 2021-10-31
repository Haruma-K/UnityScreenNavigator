#if USN_USE_TIMELINE
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    public sealed class TimelineTransitionAnimationBehaviour : TransitionAnimationBehaviour
    {
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private TimelineAsset _timelineAsset;

        public override float Duration => (float)_timelineAsset.duration;

        public override void Setup()
        {
            _director.playableAsset = _timelineAsset;
            _director.time = 0;
            _director.initialTime = 0;
            _director.playOnAwake = false;
            _director.timeUpdateMode = DirectorUpdateMode.Manual;
            _director.extrapolationMode = DirectorWrapMode.None;
        }

        public override void SetTime(float time)
        {
            _director.time = time;
            _director.Evaluate();
        }
    }
}
#endif