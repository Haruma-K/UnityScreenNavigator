using Demo.Core.Scripts.Domain.Shared.MasterRepository;
using Demo.Core.Scripts.Domain.Unit.Model;

namespace Demo.Core.Scripts.Domain.Unit.MasterRepository
{
    public interface IUnitTypeMasterTable : IMasterTable
    {
        UnitTypeMaster FindById(string id);
    }
}
