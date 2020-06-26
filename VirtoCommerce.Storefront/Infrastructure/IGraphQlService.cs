using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Cart;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public interface IGraphQlService
    {
        Task<ShoppingCartDto> SearchShoppingCartAsync(CartSearchCriteria criteria);
    }
}
