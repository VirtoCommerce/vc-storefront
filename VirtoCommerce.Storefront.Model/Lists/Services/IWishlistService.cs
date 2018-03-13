using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Lists;

namespace VirtoCommerce.Storefront.Model.Lists.Services
{
    public interface IWishlistService
    {
        Task<IPagedList<Wishlist>> SearchShoppingCartsAsync(WishlistSearchCriteria criteria);

        Task DeleteListsByIdsAsync(string[] ids);
    }
}
