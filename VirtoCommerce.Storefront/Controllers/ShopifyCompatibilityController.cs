using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Converters;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers
{
    public class ShopifyCompatibilityController : StorefrontControllerBase
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;

        public ShopifyCompatibilityController(IWorkContextAccessor workContextAccesor, IStorefrontUrlBuilder urlBuilder, ICartBuilder cartBuilder, ICatalogService catalogService)
            : base(workContextAccesor, urlBuilder)
        {
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
        }


        // GET: /cart/change?line=...&quantity=...
        [HttpGet]
        public async Task<ActionResult> Change(int line, int quantity)
        {
            EnsureCartExists();

            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ChangeItemQuantityAsync(line - 1, quantity);
                await cartBuilder.SaveAsync();
            }
            return StoreFrontRedirect("~/cart");
        }

        // POST: /cart/add
        [HttpPost]
        public async Task<ActionResult> Add(string id, int quantity)
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                var product = (await _catalogService.GetProductsAsync(new[] { id }, Model.Catalog.ItemResponseGroup.ItemLarge)).FirstOrDefault();
                if (product != null)
                {
                    await cartBuilder.AddItemAsync(product, quantity);
                    await cartBuilder.SaveAsync();
                }
            }
            return StoreFrontRedirect("~/cart");
        }

        // POST: /cart?updates=...&update=...
        [HttpPost]
        public async Task<ActionResult> Cart(int[] updates, string checkout)
        {
            string virtualRedirectUrl = "~/cart";

            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ChangeItemsQuantitiesAsync(updates);
                await cartBuilder.SaveAsync();

                StringValues dummy;
                if (Request.Form.TryGetValue("checkout", out dummy))
                {
                    virtualRedirectUrl = "~/cart/checkout";

                }
            }
            return StoreFrontRedirect(virtualRedirectUrl);
        }

        // GET: /cart/clear
        [HttpGet]
        public async Task<ActionResult> Clear()
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ClearAsync();
                await cartBuilder.SaveAsync();
            }
            return StoreFrontRedirect("~/cart");
        }

        // GET: /cart.js
        [HttpGet]
        public async Task<ActionResult> CartJs()
        {
            EnsureCartExists();
            var cartBuilder = await LoadOrCreateCartAsync();
            return LiquidJson(cartBuilder.Cart.ToShopifyModel(WorkContext.CurrentLanguage, UrlBuilder));
        }

        // POST: /cart/add.js
        [HttpPost]
        public async Task<ActionResult> AddJs(string id, int quantity = 1)
        {
            LineItem lineItem = null;

            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var product = (await _catalogService.GetProductsAsync(new[] { id }, Model.Catalog.ItemResponseGroup.ItemLarge)).FirstOrDefault();
                if (product != null)
                {
                    var cartBuilder = await LoadOrCreateCartAsync();
                    await cartBuilder.AddItemAsync(product, quantity);
                    await cartBuilder.SaveAsync();

                    lineItem = cartBuilder.Cart.Items.FirstOrDefault(i => i.ProductId == id);
                }
            }
            return LiquidJson(lineItem != null ? lineItem.ToShopifyModel(WorkContext.CurrentLanguage, UrlBuilder) : null);
        }

        // POST: /cart/change.js
        [HttpPost]
        public async Task<ActionResult> ChangeJs(string id, int quantity)
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ChangeItemQuantityAsync(id, quantity);
                await cartBuilder.SaveAsync();
                return LiquidJson(cartBuilder.Cart.ToShopifyModel(WorkContext.CurrentLanguage, UrlBuilder));
            }
        }

        // POST: /cart/update.js
        [HttpPost]
        public async Task<ActionResult> UpdateJs(int[] updates)
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ChangeItemsQuantitiesAsync(updates);
                await cartBuilder.SaveAsync();
                return LiquidJson(cartBuilder.Cart.ToShopifyModel(WorkContext.CurrentLanguage, UrlBuilder));
            }
        }

        // POST: /cart/clear.js
        [HttpPost]
        public async Task<ActionResult> ClearJs()
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ClearAsync();
                await cartBuilder.SaveAsync();
                return LiquidJson(_cartBuilder.Cart.ToShopifyModel(WorkContext.CurrentLanguage, UrlBuilder));
            }
        }

        /// GET collections
        /// This method used for display all categories
        /// <returns></returns>
        /// TODO:
        //[OutputCache(CacheProfile = "CatalogSearchCachingProfile")]
        public ActionResult Collections()
        {
            return View("list-collections", WorkContext);
        }

        private JsonResult LiquidJson(object obj)
        {
            return new JsonResult(obj, new JsonSerializerSettings
            {
                ContractResolver = new LiquidThemeEngine.Filters.RubyContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        private void EnsureCartExists()
        {
            if (WorkContext.CurrentCart == null)
            {
                throw new StorefrontException("Cart not found");
            }
        }
        private async Task<ICartBuilder> LoadOrCreateCartAsync()
        {
            var cart = WorkContext.CurrentCart.Value;
            //Need to try load fresh cart from cache or service to prevent parallel update conflict
            //because WorkContext.CurrentCart may contains old cart
            await _cartBuilder.LoadOrCreateNewTransientCartAsync(cart.Name, WorkContext.CurrentStore, cart.Customer, cart.Language, cart.Currency);
            return _cartBuilder;
        }


    }
}
