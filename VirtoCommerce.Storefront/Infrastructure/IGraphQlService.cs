using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Commands;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public interface IGraphQlService
    {
        Task<ShoppingCartDto> SearchShoppingCartAsync(CartSearchCriteria criteria);
        Task<ShoppingCartDto> AddItemToCartAsync(AddCartItemCommand command);
        Task<ShoppingCartDto> AddCouponAsync(AddCouponCommand command);
    }
}
