using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Domain.Shared.MasterRepository;

namespace Demo.Core.Scripts.Domain.Unit.MasterRepository
{
    public interface IUnitMasterRepository : IMasterRepository
    {
        UniTask<IUnitTypeMasterTable> FetchUnitTypeTableAsync();
    }
}
