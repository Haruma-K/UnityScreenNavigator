using UnityEngine;

namespace Demo.Core.Scripts.MasterRepository.FeatureFlag
{
    [CreateAssetMenu(menuName = "Demo/Master Data/Feature Flag")]
    public sealed class FeatureFlagMasterTableAsset : ScriptableObject
    {
        [SerializeField] private FeatureFlagMasterTable masterTable = new FeatureFlagMasterTable();

        public FeatureFlagMasterTable MasterTable => masterTable;
    }
}
