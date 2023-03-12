using Demo.Core.Scripts.Domain.Shared.MasterRepository;
using Demo.Core.Scripts.Domain.UnitShop.Model;

namespace Demo.Core.Scripts.Domain.UnitShop.MasterRepository
{
    public interface IUnitShopItemMasterTable : IMasterTable
    {
        UnitShopItemMaster FindById(string id);
    }
}
