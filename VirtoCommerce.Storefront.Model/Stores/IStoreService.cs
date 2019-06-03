using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Stores
{
    public interface IStoreService
    {
        Task<Store[]> GetAllStoresAsync();
        Task<Store> GetStoreByIdAsync(string id, Currency currency = null);
    }
}
