using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog.Services
{
    public interface IProductAvailabilityService
    {
        Task<bool> IsAvailable(Product product, long requestedQuantity);
        Task<long> GetAvailableQuantity(Product product);
    }
}
