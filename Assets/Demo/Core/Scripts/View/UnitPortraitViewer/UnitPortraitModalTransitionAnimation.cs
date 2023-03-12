using Demo.Core.Scripts.View.UnitTypeInformation;
using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;

namespace Demo.Core.Scripts.View.UnitPortraitViewer
{
    public class UnitPortraitModalTransitionAnimation : TransitionAnimationBehaviour
    {
        [SerializeField] private float duration = 0.3f;
        public EaseType easeType = EaseType.QuarticEaseOut;
        public bool isEnter;

        private RectTransform _targetRectTransform;
        private RectTransform _partnerImageRectTransform;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private Vector2 _startSize;
        private Vector2 _endSize;

        public override float Duration => duration;

        public override void Setup()
        {
            var portraitView = RectTransform.GetComponent<UnitPortraitViewerModal>().root.portraitView;
            _targetRectTransform = (RectTransform)portraitView.transform;
            _partnerImageRectTransform = PartnerRectTransform.GetComponent<UnitTypeInformationModal>().root
                .UnitPortraitRectTransform;
            _startPosition = isEnter ? _partnerImageRectTransform.position : _targetRectTransform.position;
            _endPosition = isEnter ? _targetRectTransform.position : _partnerImageRectTransform.position;
            _startSize = isEnter ? _partnerImageRectTransform.rect.size : _targetRectTransform.rect.size;
            _endSize = isEnter ? _targetRectTransform.rect.size : _partnerImageRectTransform.rect.size;
        }

        public override void SetTime(float time)
        {
            time = Mathf.Max(0, time);
            var progress = duration <= 0 ? 1 : Mathf.Clamp01(time / duration);
            progress = Easings.Interpolate(progress, easeType);
            var position = Vector3.Lerp(_startPosition, _endPosition, progress);
            var size = Vector2.Lerp(_startSize, _endSize, progress);
            _targetRectTransform.position = position;
            _targetRectTransform.sizeDelta = size;
            if (progress == 0 && isEnter)
                _partnerImageRectTransform.gameObject.SetActive(false);
            if (progress == 1 && !isEnter)
                _partnerImageRectTransform.gameObject.SetActive(true);
        }
    }
}
