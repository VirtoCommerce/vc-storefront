using System.Threading.Tasks;
using PagedList.Core;
using VirtoCommerce.Storefront.Model.Lists;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Model.Lists.Services
{
    public interface IWishlistService
    {
        Task<IPagedList<Wishlist>> SearchWishlistsAsync(WishlistSearchCriteria criteria);

        Task DeleteListsByIdsAsync(string[] ids);

        Task<Wishlist> CreateListAsync(Wishlist wishlis);

        Task<int> GetWishlistCountByCustomer(User customer);
    }
}
