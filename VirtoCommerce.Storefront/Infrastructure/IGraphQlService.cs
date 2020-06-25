using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Cart;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public interface IGraphQlService
    {
        Task<IEnumerable<ShoppingCartDto>> SearchShoppingCartAsync(CartSearchCriteria criteria);
    }
}
