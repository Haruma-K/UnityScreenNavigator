using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;

namespace Demo.Scripts
{
    public class CharacterImageModalTransitionAnimation : TransitionAnimationBehaviour
    {
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private EaseType _easeType = EaseType.QuarticEaseOut;
        [SerializeField] private bool _isExit;

        private RectTransform _targetRectTransform;
        private RectTransform _partnerImageRectTransform;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector2 _startSize;
        private Vector2 _endSize;

        public override float Duration => _duration;

        public override void Setup()
        {
            _targetRectTransform = RectTransform.GetComponent<CharacterImageModal>().ImageTransform;
            _partnerImageRectTransform = PartnerRectTransform.GetComponent<CharacterModal>().CharacterImageRectTransform;
            _startPosition = _isExit ? _targetRectTransform.position : _partnerImageRectTransform.position;
            _endPosition = _isExit ? _partnerImageRectTransform.position : _targetRectTransform.position;
            _startSize = _isExit ? _targetRectTransform.rect.size : _partnerImageRectTransform.rect.size;
            _endSize = _isExit ? _partnerImageRectTransform.rect.size : _targetRectTransform.rect.size;
        }

        public override void SetTime(float time)
        {
            time = Mathf.Max(0, time);
            var progress = _duration <= 0 ? 1 : Mathf.Clamp01(time / _duration);
            progress = Easings.Interpolate(progress, _easeType);
            var position = Vector3.Lerp(_startPosition, _endPosition, progress);
            var size = Vector2.Lerp(_startSize, _endSize, progress);
            _targetRectTransform.position = position;
            _targetRectTransform.sizeDelta = size;
            if (progress == 0 && !_isExit)
            {
                _partnerImageRectTransform.gameObject.SetActive(false);
            }
            if (progress == 1 && _isExit)
            {
                _partnerImageRectTransform.gameObject.SetActive(true);
            }
        }
    }
}