using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Domain.Shared.MasterRepository;

namespace Demo.Core.Scripts.Domain.FeatureFlag.MasterRepository
{
    public interface IFeatureFlagMasterRepository : IMasterRepository
    {
        UniTask<IFeatureFlagMasterTable> FetchTableAsync();
    }
}
