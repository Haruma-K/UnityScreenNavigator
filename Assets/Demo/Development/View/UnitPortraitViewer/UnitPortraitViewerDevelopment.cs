using Demo.Core.Scripts.Foundation.Common;
using Demo.Core.Scripts.View.UnitPortraitViewer;
using Demo.Development.View.Shared;
using UniRx;
using UnityEngine;

namespace Demo.Development.View.UnitPortraitViewer
{
    public sealed class UnitPortraitViewerDevelopment
        : AppViewDevelopment<UnitPortraitViewerView, UnitPortraitViewerViewState>
    {
        protected override UnitPortraitViewerViewState CreateState()
        {
            var state = new UnitPortraitViewerViewState();
            state.Portrait.ImageResourceKey.Value = ResourceKey.Textures.GetUnitPortrait("003", 1);
            state.OnCloseButtonClicked.Subscribe(_ => Debug.Log("OnCloseButtonClicked")).AddTo(this);
            return state;
        }
    }
}
