using Demo.Core.Scripts.View.Top;
using Demo.Development.View.Shared;
using UniRx;
using UnityEngine;

namespace Demo.Development.View.Top
{
    public sealed class TopDevelopment : AppViewDevelopment<TopView, TopViewState>
    {
        protected override TopViewState CreateState()
        {
            var state = new TopViewState();
            state.OnClicked.Subscribe(_ => Debug.Log("Clicked"));
            return state;
        }
    }
}