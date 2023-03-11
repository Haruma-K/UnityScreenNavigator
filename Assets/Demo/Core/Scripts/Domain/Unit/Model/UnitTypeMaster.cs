using System;
using UnityEngine;

namespace Demo.Core.Scripts.Domain.Unit.Model
{
    [Serializable]
    public sealed class UnitTypeMaster
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private string rank1Description;
        [SerializeField] private string rank2Description;
        [SerializeField] private string rank3Description;

        public string Id => id;
        public string Name => name;
        public string Rank1Description => rank1Description;
        public string Rank2Description => rank2Description;
        public string Rank3Description => rank3Description;
    }
}
