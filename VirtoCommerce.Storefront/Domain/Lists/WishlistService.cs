using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Domain.Lists;
using VirtoCommerce.Storefront.Extensions;
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

        public async Task<IPagedList<Wishlist>> SearchWishlistsAsync(WishlistSearchCriteria criteria)
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

            var cacheKey = CacheKey.With(GetType(), "SearchWishlistsAsync", criteria.GetHashCode().ToString(), criteria.StoreId, criteria.Language.CultureName, criteria.Currency.Code);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(WishlistCacheRegion.CreateSearchResultsChangeToken(criteria.Customer.Id));

                var workContext = _workContextAccessor.WorkContext;
                var resultDto = await _cartApi.SearchAsync(criteria.ToSearchCriteriaDto());
                var result = resultDto.Results.Select(x => x.ToWishlist(criteria.Currency, criteria.Language, criteria.Customer)).ToList();
                return new StaticPagedList<Wishlist>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount.Value);
            });
        }

        public async Task<Wishlist> CreateListAsync(Wishlist wishlis)
        {
            var list = await _cartApi.CreateAsync(wishlis.ToShoppingCartDto());
            WishlistCacheRegion.ExpireSearchResults(wishlis.CustomerId);

            return list.ToWishlist(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task DeleteListsByIdsAsync(string[] ids)
        {
            await _cartApi.DeleteCartsAsync(ids);

            foreach (var id in ids)
            {
                WishlistCacheRegion.ExpireSearchResults(_workContextAccessor.WorkContext.CurrentUser.Id);
                CartCacheRegion.ExpireCart(new ShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage) { Id = id });
            }
        }
    }
}
