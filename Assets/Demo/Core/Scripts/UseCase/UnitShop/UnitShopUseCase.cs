using System.Linq;
using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.APIGateway.UnitShop;
using Demo.Core.Scripts.Domain.UnitShop.Model;

namespace Demo.Core.Scripts.UseCase.UnitShop
{
    public sealed class UnitShopUseCase
    {
        private readonly UnitShopAPIGateway _apiGateway;

        public UnitShopUseCase(UnitShopItemSet model, UnitShopAPIGateway apiGateway)
        {
            Model = model;
            _apiGateway = apiGateway;
        }

        public UnitShopItemSet Model { get; }

        public async UniTask FetchItemSetAsync()
        {
            var response = await _apiGateway.FetchItemSetAsync();

            var regularItems = response.RegularItems
                .Select(x => new UnitShopItem(x.Id, x.MasterId, x.IsSoldOut))
                .ToArray();
            var specialItems = response.SpecialItems
                .Select(x => new UnitShopItem(x.Id, x.MasterId, x.IsSoldOut))
                .ToArray();
            var saleItems = response.SaleItems
                .Select(x => new UnitShopItem(x.Id, x.MasterId, x.IsSoldOut))
                .ToArray();

            Model.SetItems(regularItems, specialItems, saleItems);
        }

        public async UniTask PurchaseItemAsync(PurchaseItemRequest request)
        {
            var unitShopItemId = request.UnitShopItemId;
            await _apiGateway.PurchaseItemAsync(unitShopItemId);
            Model.RegularItems
                .Concat(Model.SpecialItems)
                .Concat(Model.SaleItems)
                .First(x => x.Id == unitShopItemId)
                .SetValue(true);
        }

        #region Requests

        public readonly struct PurchaseItemRequest
        {
            public PurchaseItemRequest(string unitShopItemId)
            {
                UnitShopItemId = unitShopItemId;
            }

            public string UnitShopItemId { get; }
        }

        #endregion
    }
}
