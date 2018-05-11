using System.Collections.Generic;
using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Cart.Services
{
    public interface ICartService
    {
        Task<IPagedList<ShoppingCart>> SearchCartsAsync(CartSearchCriteria criteria);
        Task<ShoppingCart> SaveChanges(ShoppingCart cart);
        Task<ShoppingCart> GetByIdAsync(string cartId);
        Task DeleteCartByIdAsync(string cartId);

        Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(ShoppingCart cart);
        Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(ShoppingCart cart);

    }
}
