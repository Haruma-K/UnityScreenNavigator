using System;
using System.Collections.Generic;
using System.Linq;
using Demo.Core.Scripts.Domain.FeatureFlag.MasterRepository;
using Demo.Core.Scripts.Domain.FeatureFlag.Model;
using UnityEngine;

namespace Demo.Core.Scripts.MasterRepository.FeatureFlag
{
    [Serializable]
    public sealed class FeatureFlagMasterTable : IFeatureFlagMasterTable
    {
        [SerializeField] private List<FeatureFlagMaster> items = new List<FeatureFlagMaster>();

        [NonSerialized] private bool _isInitialized;
        private Dictionary<string, FeatureFlagMaster> _items;

        public FeatureFlagMaster FindById(string id)
        {
            if (!_isInitialized)
                throw new InvalidOperationException(
                    $"{nameof(FeatureFlagMasterTable)} is not initialized. Call {nameof(Initialize)}() first.");

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
