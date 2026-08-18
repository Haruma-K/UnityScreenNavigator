using System;
using R3;
using UnityEngine;

namespace Demo.Core.Scripts.View.Foundation.Binders
{
    public static class GamObjectExtensions
    {
        public static IDisposable SetActiveSelfSource(this GameObject self, Observable<bool> source,
            bool invert = false)
        {
            return source
                .Subscribe(x =>
                {
                    x = invert ? !x : x;
                    self.SetActive(x);
                })
                .AddTo(self);
        }
    }
}
