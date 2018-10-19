using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Stores
{
    public interface IStoreService
    {
        Task<Store[]> GetAllStoresAsync();
    }
}
