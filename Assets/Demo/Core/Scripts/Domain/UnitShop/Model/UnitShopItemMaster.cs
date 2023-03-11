using System;
using UnityEngine;

namespace Demo.Core.Scripts.Domain.UnitShop.Model
{
    [Serializable]
    public sealed class UnitShopItemMaster
    {
        [SerializeField] private string id;
        [SerializeField] private int cost;
        [SerializeField] private string unitTypeMasterId;

        public string Id => id;
        public int Cost => cost;
        public string UnitTypeMasterId => unitTypeMasterId;
    }
}
