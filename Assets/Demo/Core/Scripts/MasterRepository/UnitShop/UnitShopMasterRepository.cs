using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Domain.UnitShop.MasterRepository;
using UnityEngine.AddressableAssets;

namespace Demo.Core.Scripts.MasterRepository.UnitShop
{
    public sealed class UnitShopMasterRepository : IUnitShopMasterRepository
    {
        private IUnitShopItemMasterTable _itemTable;

        public void ClearCache()
        {
            _itemTable = null;
        }

        public async UniTask<IUnitShopItemMasterTable> FetchItemTableAsync()
        {
            if (_itemTable != null)
                return _itemTable;

            var asset =
                await Addressables.LoadAssetAsync<UnitShopItemMasterTableAsset>("UnitShopItemMasterTableAsset");
            _itemTable = asset.MasterTable;
            _itemTable.Initialize();
            return _itemTable;
        }
    }
}
