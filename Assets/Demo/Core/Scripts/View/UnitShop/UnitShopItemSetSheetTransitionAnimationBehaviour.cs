using UnityEngine;
using UnityScreenNavigator.Runtime.Core.Shared;

namespace Demo.Core.Scripts.View.UnitShop
{
    public sealed class UnitShopItemSetSheetTransitionAnimationBehaviour : TransitionAnimationBehaviour
    {
        [SerializeField] private float duration = 0.2f;
        public bool isEnterAnimation;
        public EaseType easeType = EaseType.QuarticEaseOut;

        private Vector3 _afterPosition;
        private Vector3 _beforePosition;
        private int _partnerTabIndex;

        private int _tabIndex;

        public override float Duration => duration;

        public override void SetTime(float time)
        {
            var progress = duration <= 0.0f ? 1.0f : Mathf.Clamp01(time / duration);
            progress = Easings.Interpolate(progress, easeType);
            var position = Vector3.Lerp(_beforePosition, _afterPosition, progress);
            RectTransform.anchoredPosition = position;
        }

        public override void Setup()
        {
            var sheet = RectTransform.GetComponent<UnitShopItemSetSheet>();
            var partnerSheet = PartnerRectTransform.GetComponent<UnitShopItemSetSheet>();
            _tabIndex = sheet.TabIndex;
            _partnerTabIndex = partnerSheet.TabIndex;

            SheetAlignment beforeAlignment;
            SheetAlignment afterAlignment;
            if (isEnterAnimation)
            {
                beforeAlignment = _tabIndex < _partnerTabIndex ? SheetAlignment.Left : SheetAlignment.Right;
                afterAlignment = SheetAlignment.Center;
            }
            else
            {
                beforeAlignment = SheetAlignment.Center;
                afterAlignment = _tabIndex < _partnerTabIndex ? SheetAlignment.Left : SheetAlignment.Right;
            }

            _beforePosition = beforeAlignment.ToPosition(RectTransform);
            _afterPosition = afterAlignment.ToPosition(RectTransform);
        }
    }
}
