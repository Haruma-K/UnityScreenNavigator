using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Core.Scripts.Domain.UnitShop.MasterRepository;
using Demo.Core.Scripts.Domain.UnitShop.Model;
using UnityEngine;

namespace Demo.Core.Scripts.MasterRepository.UnitShop
{
    [Serializable]
    public sealed class UnitShopItemMasterTable : IUnitShopItemMasterTable
    {
        [SerializeField] private List<UnitShopItemMaster> items = new List<UnitShopItemMaster>();

        [NonSerialized] private bool _isInitialized;
        private Dictionary<string, UnitShopItemMaster> _items;

        public UnitShopItemMaster FindById(string id)
        {
            return !_items.TryGetValue(id, out var item) ? null : item;
        }

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _items = items.ToDictionary(x => x.Id);

            _isInitialized = true;
        }
    }
}
