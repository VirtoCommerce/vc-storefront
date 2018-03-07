using System.Threading.Tasks;
using PagedList.Core;

namespace VirtoCommerce.Storefront.Model.Cart.Services
{
    public interface ICartService
    {
        Task<IPagedList<ShoppingCart>> SearchShoppingCartsAsync(ShoppingCartSearchCriteria criteria);

        Task DeleteCartsByIdsAsync(string[] ids);
    }
}
