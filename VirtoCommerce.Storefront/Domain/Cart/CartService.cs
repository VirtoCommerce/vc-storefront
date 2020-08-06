using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Contracts;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Cart
{
    public class CartService : ICartService
    {
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly ICartModule _cartApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IGraphQLClient _client;
        private readonly UserManager<User> _userManager;

        public CartService(ICartModule cartModule, IWorkContextAccessor workContextAccessor,
            IStorefrontMemoryCache memoryCache, IGraphQLClient client, UserManager<User> userManager)
        {
            _cartApi = cartModule;
            _memoryCache = memoryCache;
            _client = client;
            _workContextAccessor = workContextAccessor;
            _userManager = userManager;
        }

        public async Task DeleteCartByIdAsync(string cartId)
        {
            if (cartId == null)
            {
                throw new ArgumentNullException(nameof(cartId));
            }
            await _cartApi.DeleteCartsAsync(new[] { cartId });
        }

        public async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            var result = Enumerable.Empty<PaymentMethod>();
            if (!cart.IsTransient())
            {
                var payments = await _cartApi.GetAvailablePaymentMethodsAsync(cart.Id);
                result = payments.Select(x => x.ToCartPaymentMethod(cart)).OrderBy(x => x.Priority).ToList();
            }
            return result;
        }

        public virtual async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            var result = Enumerable.Empty<ShippingMethod>();
            if (!cart.IsTransient())
            {
                var shippingRates = await _cartApi.GetAvailableShippingRatesAsync(cart.Id);
                result = shippingRates.Select(x => x.ToShippingMethod(cart.Currency, _workContextAccessor.WorkContext.AllCurrencies)).OrderBy(x => x.Priority).ToList();
            }
            return result;
        }

        public async Task<ShoppingCart> GetByIdAsync(string cartId)
        {
            ShoppingCart result = null;
            var cartDto = await _cartApi.GetCartByIdAsync(cartId);
            if (cartDto != null)
            {
                var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(cartDto.Currency));
                var language = string.IsNullOrEmpty(cartDto.LanguageCode) ? Language.InvariantLanguage : new Language(cartDto.LanguageCode);
                result = cartDto.ToShoppingCart(currency, language, await _userManager.FindByIdAsync(cartDto.CustomerId));
            }
            return result;
        }

        public virtual async Task<ShoppingCart> SaveChanges(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            var cartDto = cart.ToShoppingCartDto();
            if (string.IsNullOrEmpty(cartDto.Id))
            {
                cartDto = await _cartApi.CreateAsync(cartDto);
            }
            else
            {
                cartDto = await _cartApi.UpdateShoppingCartAsync(cartDto);
            }
            var result = cartDto.ToShoppingCart(cart.Currency, cart.Language, cart.Customer);
            return result;
        }

        public virtual async Task<IPagedList<ShoppingCart>> SearchCartsAsync(CartSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            var cacheKey = CacheKey.With(GetType(), "SearchCartsAsync", criteria.GetCacheKey());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CartCacheRegion.CreateCustomerChangeToken(criteria.Customer?.Id));

                var shoppingCarts = new List<ShoppingCart>();
                var response = await InnerSearchCartsAsync(criteria);

                foreach (var cartDto in response.Carts.Items)
                {
                    var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(cartDto.Currency));
                    var language = cartDto.Language ?? Language.InvariantLanguage;
                    var user = await _userManager.FindByIdAsync(cartDto.CustomerId) ?? criteria.Customer;
                    shoppingCarts.Add(cartDto.ToShoppingCart(currency, language, user));
                }

                return new StaticPagedList<ShoppingCart>(shoppingCarts, criteria.PageNumber, criteria.PageSize, response.Carts.TotalCount);
            });
        }

        private async Task<SearchCartResponseDto> InnerSearchCartsAsync(CartSearchCriteria criteria)
        {
            var query = QueryHelper.SearchCarts(storeId: criteria.StoreId,
                userId: criteria.Customer.Id,
                cultureName: criteria.Language?.CultureName ?? "en-US",
                currencyCode: criteria.Currency.Code,
                type: criteria.Type,
                sort: criteria.Sort,
                skip: (criteria.PageNumber - 1) * criteria.PageSize,
                take: criteria.PageSize);
            var result = await _client.SendQueryAsync<SearchCartResponseDto>(new GraphQLRequest { Query = query });
            return result.Data;
        }
    }
}
