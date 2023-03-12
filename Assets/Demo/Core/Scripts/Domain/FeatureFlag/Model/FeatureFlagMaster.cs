using System;
using UnityEngine;

namespace Demo.Core.Scripts.Domain.FeatureFlag.Model
{
    [Serializable]
    public sealed class FeatureFlagMaster
    {
        [SerializeField] private string id;
        [SerializeField] private bool enabled;

        public string Id => id;
        public bool Enabled => enabled;
    }
}
