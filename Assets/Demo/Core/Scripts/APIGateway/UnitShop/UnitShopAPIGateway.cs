using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Demo.Core.Scripts.APIGateway.UnitShop
{
    public sealed class UnitShopAPIGateway
    {
        // Use on memory cache because this is just a demo.
        private readonly HashSet<string> _purchasedItemIds = new HashSet<string>();

        public UniTask<FetchItemSetResponse> FetchItemSetAsync()
        {
            return UniTask.FromResult(CreateDummyData());
        }

        public async UniTask PurchaseItemAsync(string unitShopItemId)
        {
            // Simulate network delay.
            await UniTask.Delay(1000);
            _purchasedItemIds.Add(unitShopItemId);
        }

        private FetchItemSetResponse CreateDummyData()
        {
            var regularItems = new List<FetchItemSetResponse.UnitShopItem>
            {
                new FetchItemSetResponse.UnitShopItem
                (
                    "unit_shop_item_0001",
                    "unit_shop_item_0001",
                    _purchasedItemIds.Contains("unit_shop_item_0001")
                )
            };

            var specialItems = new List<FetchItemSetResponse.UnitShopItem>
            {
                new FetchItemSetResponse.UnitShopItem
                (
                    "unit_shop_item_0002",
                    "unit_shop_item_0002",
                    _purchasedItemIds.Contains("unit_shop_item_0002")
                )
            };

            var saleItems = new List<FetchItemSetResponse.UnitShopItem>
            {
                new FetchItemSetResponse.UnitShopItem
                (
                    "unit_shop_item_0003",
                    "unit_shop_item_0003",
                    _purchasedItemIds.Contains("unit_shop_item_0003")
                )
            };

            var itemSet = new FetchItemSetResponse(regularItems, specialItems, saleItems);
            return itemSet;
        }

        #region Responses

        public readonly struct FetchItemSetResponse
        {
            public FetchItemSetResponse(IReadOnlyList<UnitShopItem> regularItems,
                IReadOnlyList<UnitShopItem> specialItems, IReadOnlyList<UnitShopItem> saleItems)
            {
                RegularItems = regularItems;
                SpecialItems = specialItems;
                SaleItems = saleItems;
            }

            public IReadOnlyList<UnitShopItem> RegularItems { get; }
            public IReadOnlyList<UnitShopItem> SpecialItems { get; }
            public IReadOnlyList<UnitShopItem> SaleItems { get; }


            public readonly struct UnitShopItem
            {
                public UnitShopItem(string id, string masterId, bool isSoldOut)
                {
                    Id = id;
                    MasterId = masterId;
                    IsSoldOut = isSoldOut;
                }

                public string Id { get; }
                public string MasterId { get; }
                public bool IsSoldOut { get; }
            }
        }

        #endregion
    }
}
