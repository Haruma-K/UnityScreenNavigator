using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Domain.Unit.MasterRepository;
using UnityEngine.AddressableAssets;

namespace Demo.Core.Scripts.MasterRepository.Unit
{
    public sealed class UnitMasterRepository : IUnitMasterRepository
    {
        private IUnitTypeMasterTable _table;

        public void ClearCache()
        {
            _table = null;
        }

        public async UniTask<IUnitTypeMasterTable> FetchUnitTypeTableAsync()
        {
            if (_table != null)
                return _table;

            var asset =
                await Addressables.LoadAssetAsync<UnitTypeMasterTableAsset>("UnitTypeMasterTableAsset");
            _table = asset.MasterTable;
            _table.Initialize();
            return _table;
        }
    }
}
