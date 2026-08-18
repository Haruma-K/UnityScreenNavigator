using System;
using R3;
using UnityEngine.UI;

namespace Demo.Core.Scripts.View.Foundation.Binders
{
    public static class ToggleExtensions
    {
        public static IDisposable SetOnValueChangedDestination(this Toggle self, Action<bool> onValueChanged)
        {
            return self.onValueChanged
                .AsObservable()
                .Subscribe(onValueChanged.Invoke)
                .AddTo(self);
        }
    }
}
