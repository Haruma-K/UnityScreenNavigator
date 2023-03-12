using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Domain.FeatureFlag.MasterRepository;
using UnityEngine.AddressableAssets;

namespace Demo.Core.Scripts.MasterRepository.FeatureFlag
{
    public sealed class FeatureFlagMasterRepository : IFeatureFlagMasterRepository
    {
        private IFeatureFlagMasterTable _table;

        public void ClearCache()
        {
            _table = null;
        }

        public async UniTask<IFeatureFlagMasterTable> FetchTableAsync()
        {
            if (_table != null)
                return _table;

            var asset =
                await Addressables.LoadAssetAsync<FeatureFlagMasterTableAsset>("FeatureFlagMasterTableAsset");
            _table = asset.MasterTable;
            _table.Initialize();
            return _table;
        }
    }
}
