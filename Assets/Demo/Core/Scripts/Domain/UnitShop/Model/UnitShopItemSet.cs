using System;
using System.Collections.Generic;
using UniRx;

namespace Demo.Core.Scripts.Domain.UnitShop.Model
{
    public sealed class UnitShopItemSet
    {
        private readonly Subject<ItemsChangedEvent> _itemsChangedSubject = new Subject<ItemsChangedEvent>();

        private readonly List<UnitShopItem> _regularItems = new List<UnitShopItem>();
        private readonly List<UnitShopItem> _saleItems = new List<UnitShopItem>();
        private readonly List<UnitShopItem> _specialItems = new List<UnitShopItem>();

        public IReadOnlyList<UnitShopItem> RegularItems => _regularItems;
        public IReadOnlyList<UnitShopItem> SpecialItems => _specialItems;
        public IReadOnlyList<UnitShopItem> SaleItems => _saleItems;

        public IObservable<ItemsChangedEvent> ItemsChanged => _itemsChangedSubject;

        internal void SetItems(IReadOnlyList<UnitShopItem> regularItems, IReadOnlyList<UnitShopItem> specialItems,
            IReadOnlyList<UnitShopItem> saleItems)
        {
            _regularItems.Clear();
            _regularItems.AddRange(regularItems);

            _specialItems.Clear();
            _specialItems.AddRange(specialItems);

            _saleItems.Clear();
            _saleItems.AddRange(saleItems);

            _itemsChangedSubject.OnNext(new ItemsChangedEvent(regularItems, specialItems, saleItems));
        }

        public readonly struct ItemsChangedEvent
        {
            public ItemsChangedEvent(IReadOnlyList<UnitShopItem> regularItems, IReadOnlyList<UnitShopItem> specialItems
                , IReadOnlyList<UnitShopItem> saleItems)
            {
                RegularItems = regularItems;
                SpecialItems = specialItems;
                SaleItems = saleItems;
            }

            public IReadOnlyList<UnitShopItem> RegularItems { get; }
            public IReadOnlyList<UnitShopItem> SpecialItems { get; }
            public IReadOnlyList<UnitShopItem> SaleItems { get; }
        }
    }
}
