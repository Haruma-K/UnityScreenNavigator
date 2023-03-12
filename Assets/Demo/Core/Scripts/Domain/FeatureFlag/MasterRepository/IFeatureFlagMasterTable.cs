using Demo.Core.Scripts.Domain.FeatureFlag.Model;
using Demo.Core.Scripts.Domain.Shared.MasterRepository;

namespace Demo.Core.Scripts.Domain.FeatureFlag.MasterRepository
{
    public interface IFeatureFlagMasterTable : IMasterTable
    {
        FeatureFlagMaster FindById(string id);
    }
}
