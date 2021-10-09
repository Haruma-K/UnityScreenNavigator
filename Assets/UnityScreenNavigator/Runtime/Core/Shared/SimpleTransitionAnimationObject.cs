using UnityEngine;

namespace UnityScreenNavigator.Runtime.Core.Shared
{
    [CreateAssetMenu(menuName = "Screen Navigator/Simple Transition Animation")]
    public sealed class SimpleTransitionAnimationObject : TransitionAnimationObject
    {
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private EaseType _easeType = EaseType.QuarticEaseOut;
        [SerializeField] private SheetAlignment _beforeAlignment = SheetAlignment.Center;
        [SerializeField] private Vector3 _beforeScale = Vector3.one;
        [SerializeField] private float _beforeAlpha = 1.0f;
        [SerializeField] private SheetAlignment _afterAlignment = SheetAlignment.Center;
        [SerializeField] private Vector3 _afterScale = Vector3.one;
        [SerializeField] private float _afterAlpha = 1.0f;

        private Vector3 _afterPosition;
        private Vector3 _beforePosition;
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        public override float Duration => _duration;

        public static SimpleTransitionAnimationObject CreateInstance(float? duration = null, EaseType? easeType = null,
            SheetAlignment? beforeAlignment = null, Vector3? beforeScale = null, float? beforeAlpha = null,
            SheetAlignment? afterAlignment = null, Vector3? afterScale = null, float? afterAlpha = null)
        {
            var anim = CreateInstance<SimpleTransitionAnimationObject>();
            anim.SetParams(duration, easeType, beforeAlignment, beforeScale, beforeAlpha, afterAlignment, afterScale,
                afterAlpha);
            return anim;
        }

        public override void Setup(RectTransform sheet)
        {
            _rectTransform = (RectTransform)sheet.transform;
            _beforePosition = _beforeAlignment.ToPosition(_rectTransform);
            _afterPosition = _afterAlignment.ToPosition(_rectTransform);
            if (!sheet.gameObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = sheet.gameObject.AddComponent<CanvasGroup>();
            }

            _canvasGroup = canvasGroup;
        }

        public override void SetTime(float time)
        {
            var progress = _duration <= 0.0f ? 0.0f : Mathf.Clamp01(time / _duration);
            progress = Easings.Interpolate(progress, _easeType);
            var position = Vector3.Lerp(_beforePosition, _afterPosition, progress);
            var scale = Vector3.Lerp(_beforeScale, _afterScale, progress);
            var alpha = Mathf.Lerp(_beforeAlpha, _afterAlpha, progress);
            _rectTransform.anchoredPosition = position;
            _rectTransform.localScale = scale;
            _canvasGroup.alpha = alpha;
        }

        public void SetParams(float? duration = null, EaseType? easeType = null, SheetAlignment? beforeAlignment = null,
            Vector3? beforeScale = null, float? beforeAlpha = null, SheetAlignment? afterAlignment = null,
            Vector3? afterScale = null, float? afterAlpha = null)
        {
            if (duration.HasValue)
            {
                _duration = duration.Value;
            }

            if (easeType.HasValue)
            {
                _easeType = easeType.Value;
            }

            if (beforeAlignment.HasValue)
            {
                _beforeAlignment = beforeAlignment.Value;
            }

            if (beforeScale.HasValue)
            {
                _beforeScale = beforeScale.Value;
            }

            if (beforeAlpha.HasValue)
            {
                _beforeAlpha = beforeAlpha.Value;
            }

            if (afterAlignment.HasValue)
            {
                _afterAlignment = afterAlignment.Value;
            }

            if (afterScale.HasValue)
            {
                _afterScale = afterScale.Value;
            }

            if (afterAlpha.HasValue)
            {
                _afterAlpha = afterAlpha.Value;
            }
        }
    }
}