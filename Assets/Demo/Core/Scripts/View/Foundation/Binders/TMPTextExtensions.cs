using System;
using TMPro;
using UniRx;

namespace Demo.Core.Scripts.View.Foundation.Binders
{
    public static class TMPTextExtensions
    {
        public static IDisposable SetTextSource(this TMP_Text self, IObservable<string> source)
        {
            return source
                .Subscribe(x => { self.text = x; })
                .AddTo(self);
        }

        public static IDisposable SetTextSource(this TMP_Text self, IObservable<int> source,
            Func<int, string> converter = null)
        {
            return source
                .Subscribe(x =>
                {
                    var text = converter == null ? x.ToString() : converter(x);
                    self.text = text;
                })
                .AddTo(self);
        }
    }
}
