using UnityEngine;

namespace Demo.Core.Scripts.MasterRepository.Unit
{
    [CreateAssetMenu(menuName = "Demo/Master Data/Unit Type")]
    public sealed class UnitTypeMasterTableAsset : ScriptableObject
    {
        [SerializeField] private UnitTypeMasterTable masterTable = new UnitTypeMasterTable();

        public UnitTypeMasterTable MasterTable => masterTable;
    }
}
