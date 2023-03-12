using UnityEngine;

namespace Demo.Core.Scripts.MasterRepository.UnitShop
{
    [CreateAssetMenu(menuName = "Demo/Master Data/Unit Shop Item")]
    public sealed class UnitShopItemMasterTableAsset : ScriptableObject
    {
        [SerializeField] private UnitShopItemMasterTable masterTable = new UnitShopItemMasterTable();

        public UnitShopItemMasterTable MasterTable => masterTable;
    }
}
