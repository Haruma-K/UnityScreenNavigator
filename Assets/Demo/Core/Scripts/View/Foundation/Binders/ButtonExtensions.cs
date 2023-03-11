using System;
using UniRx;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Foundation.Binders
{
    public static class ButtonExtensions
    {
        public static IDisposable SetOnClickDestination(this Button self, Action onClick)
        {
            return self.onClick
                .AsObservable()
                .Subscribe(x => onClick.Invoke())
                .AddTo(self);
        }
    }
}
