using System;
using UniRx;

namespace Demo.Core.Scripts.Domain.UnitShop.Model
{
    public sealed class UnitShopItem
    {
        private readonly Subject<ValueChangedEvent> _valueChangedSubject = new Subject<ValueChangedEvent>();

        public UnitShopItem(string id, string masterId, bool isSoldOut)
        {
            Id = id;
            MasterId = masterId;
            IsSoldOut = isSoldOut;
        }

        public string Id { get; }
        public string MasterId { get; }
        public bool IsSoldOut { get; private set; }

        public IObservable<ValueChangedEvent> ValueChanged => _valueChangedSubject;

        internal void SetValue(bool isSoldOut)
        {
            IsSoldOut = isSoldOut;
            _valueChangedSubject.OnNext(new ValueChangedEvent(isSoldOut));
        }

        public readonly struct ValueChangedEvent
        {
            public ValueChangedEvent(bool isSoldOut)
            {
                IsSoldOut = isSoldOut;
            }

            public bool IsSoldOut { get; }
        }
    }
}
