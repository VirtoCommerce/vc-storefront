using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Domain.Lists;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Lists;
using VirtoCommerce.Storefront.Model.Lists.Services;

namespace VirtoCommerce.Storefront.Domain.Cart
{
    public class WishlistService : IWishlistService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICartModule _cartApi;
        private readonly IWorkContextAccessor _workContextAccessor;

        public WishlistService(ICartModule cartModule, IWorkContextAccessor workContextAccessor, IMemoryCache memoryCache)
        {
            _cartApi = cartModule;
            _memoryCache = memoryCache;
            _workContextAccessor = workContextAccessor;
        }

        public async Task<IPagedList<Wishlist>> SearchShoppingCartsAsync(WishlistSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            if (criteria?.Language == null)
            {
                throw new ArgumentNullException(nameof(criteria.Language));
            }

            if (criteria?.Currency == null)
            {
                throw new ArgumentNullException(nameof(criteria.Currency));
            }

            if (criteria?.Customer == null)
            {
                throw new ArgumentNullException(nameof(criteria.Customer));
            }

            var workContext = _workContextAccessor.WorkContext;
            var resultDto = await _cartApi.SearchAsync(criteria.ToSearchCriteriaDto());
            var result = resultDto.Results.Select(x => x.ToWishlist(criteria.Currency, criteria.Language, criteria.Customer)).ToList();
            return new StaticPagedList<Wishlist>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount.Value);
        }

        public async Task DeleteListsByIdsAsync(string[] ids)
        {
            await _cartApi.DeleteCartsAsync(ids);

            foreach (var id in ids)
            {
                CartCacheRegion.ExpireCart(new ShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage) { Id = id });
            }
        }
    }
}
