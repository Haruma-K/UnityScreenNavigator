using Cysharp.Threading.Tasks;
using Demo.Core.Scripts.Domain.Shared.MasterRepository;

namespace Demo.Core.Scripts.Domain.UnitShop.MasterRepository
{
    public interface IUnitShopMasterRepository : IMasterRepository
    {
        UniTask<IUnitShopItemMasterTable> FetchItemTableAsync();
    }
}
