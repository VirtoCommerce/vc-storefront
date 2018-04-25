using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Cart
{
    public class CartService : ICartService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICartModule _cartApi;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly UserManager<User> _userManager;

        public CartService(ICartModule cartModule, IWorkContextAccessor workContextAccessor, IMemoryCache memoryCache, UserManager<User> userManager)
        {
            _cartApi = cartModule;
            _memoryCache = memoryCache;
            _workContextAccessor = workContextAccessor;
            _userManager = userManager;
        }      

        public async Task DeleteCartByIdAsync(string cartId)
        {
            if(cartId == null)
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
            var payments = await _cartApi.GetAvailablePaymentMethodsAsync(cart.Id);
            var result = payments.Select(x => x.ToPaymentMethod(cart)).OrderBy(x => x.Priority).ToList();
            return result;
        }

        public async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync(ShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }
            var shippingRates = await _cartApi.GetAvailableShippingRatesAsync(cart.Id);
            var result = shippingRates.Select(x => x.ToShippingMethod(cart.Currency, _workContextAccessor.WorkContext.AllCurrencies)).OrderBy(x => x.Priority).ToList();
            return result;
        }

        public async Task<ShoppingCart> GetByIdAsync(string id)
        {
            ShoppingCart result = null;
            var cartDto = await _cartApi.GetCartByIdAsync(id);
            if(cartDto != null)
            {
                var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(cartDto.Currency));
                var language = string.IsNullOrEmpty(cartDto.LanguageCode) ? Language.InvariantLanguage : new Language(cartDto.LanguageCode);
                result = cartDto.ToShoppingCart(currency, language, await _userManager.FindByIdAsync(cartDto.CustomerId));
            }
            return result;
        }

        public async Task<ShoppingCart> SaveChanges(ShoppingCart cart)
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
                cartDto = await _cartApi.UpdateAsync(cartDto);
            }
            var result = cartDto.ToShoppingCart(cart.Currency, cart.Language, cart.Customer);
            return result;
        }

        public async Task<IPagedList<ShoppingCart>> SearchCartsAsync(CartSearchCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }
            var cacheKey = CacheKey.With(GetType(), "SearchCartsAsync", criteria.GetHashCode().ToString());
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(CartCacheRegion.CreateCustomerChangeToken(criteria.Customer?.Id));

                var workContext = _workContextAccessor.WorkContext;
                var resultDto = await _cartApi.SearchAsync(criteria.ToSearchCriteriaDto());               
                var result = new List<ShoppingCart>();
                foreach(var cartDto in resultDto.Results)
                {
                    var currency = _workContextAccessor.WorkContext.AllCurrencies.FirstOrDefault(x => x.Equals(cartDto.Currency));
                    var language = string.IsNullOrEmpty(cartDto.LanguageCode) ? Language.InvariantLanguage : new Language(cartDto.LanguageCode);
                    var user = await _userManager.FindByIdAsync(cartDto.CustomerId) ?? criteria.Customer;
                    var cart = cartDto.ToShoppingCart(currency, language, user);
                    result.Add(cart);
                }              
                return new StaticPagedList<ShoppingCart>(result, criteria.PageNumber, criteria.PageSize, resultDto.TotalCount.Value);
            });
        }   

       
    }
}
