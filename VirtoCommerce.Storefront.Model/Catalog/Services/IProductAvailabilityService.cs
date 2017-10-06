using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Catalog.Services
{
    public interface IProductAvailabilityService
    {
        Task<bool> IsAvailable(Product product, long requestedQuantity);
        bool IsBuyable(Product product);
        Task<bool> IsInStock(Product product);
        Task<long> GetAvailableQuantity(Product product);
    }
}
