using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Core.Scripts.Domain.Unit.MasterRepository;
using Demo.Core.Scripts.Domain.Unit.Model;
using UnityEngine;

namespace Demo.Core.Scripts.MasterRepository.Unit
{
    [Serializable]
    public sealed class UnitTypeMasterTable : IUnitTypeMasterTable
    {
        [SerializeField] private List<UnitTypeMaster> items = new List<UnitTypeMaster>();
        [NonSerialized] private bool _isInitialized;
        private Dictionary<string, UnitTypeMaster> _items;

        public UnitTypeMaster FindById(string id)
        {
            if (!_isInitialized)
                throw new InvalidOperationException(
                    $"{nameof(UnitTypeMasterTable)} is not initialized. Call {nameof(Initialize)}() first.");

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
