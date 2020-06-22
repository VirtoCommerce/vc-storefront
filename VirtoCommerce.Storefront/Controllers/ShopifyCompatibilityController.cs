using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine.Converters;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
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
        [HttpGet("cart/change")]
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
        [HttpPost("cart/add")]
        public async Task<ActionResult> Add(string id, int quantity)
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                var product = (await _catalogService.GetProductsAsync(new[] { id }, Model.Catalog.ItemResponseGroup.ItemLarge)).FirstOrDefault();
                if (product != null)
                {
                    await cartBuilder.AddItemAsync(new AddCartItem { Product = product, Quantity = quantity });
                    await cartBuilder.SaveAsync();
                }
            }
            return StoreFrontRedirect("~/cart");
        }

        // POST: /cart?updates=...&update=...
        [HttpPost("cart")]
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
        [HttpGet("cart/clear")]
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
        [HttpGet("cart.js")]
        public async Task<ActionResult> CartJs()
        {
            EnsureCartExists();
            var cartBuilder = await LoadOrCreateCartAsync();
            return LiquidJson(cartBuilder.Cart);
        }

        // POST: /cart/add.js
        [HttpPost("cart/add.js")]
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
                    await cartBuilder.AddItemAsync(new AddCartItem { Product = product, Quantity = quantity });
                    await cartBuilder.SaveAsync();

                    lineItem = cartBuilder.Cart.Items.FirstOrDefault(i => i.ProductId == id);
                }
            }
            return LiquidJson(lineItem);
        }

        // POST: /cart/change.js
        [HttpPost("cart/change.js")]
        public async Task<ActionResult> ChangeJs(string id, int quantity)
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ChangeItemQuantityAsync(new ChangeCartItemQty { LineItemId = id, Quantity = quantity });
                await cartBuilder.SaveAsync();
                return LiquidJson(cartBuilder.Cart);
            }
        }

        // POST: /cart/update.js
        [HttpPost("cart/update.js")]
        public async Task<ActionResult> UpdateJs(int[] updates)
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ChangeItemsQuantitiesAsync(updates);
                await cartBuilder.SaveAsync();
                return LiquidJson(cartBuilder.Cart);
            }
        }

        // POST: /cart/clear.js
        [HttpPost("cart/clear.js")]
        public async Task<ActionResult> ClearJs()
        {
            EnsureCartExists();
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ClearAsync();
                await cartBuilder.SaveAsync();
                return LiquidJson(_cartBuilder.Cart);
            }
        }

        /// GET collections
        /// This method used for display all categories
        /// <returns></returns>
        /// TODO:
        //[OutputCache(CacheProfile = "CatalogSearchCachingProfile")]
        [HttpGet("collections")]
        public ActionResult Collections()
        {
            return View("list-collections", WorkContext);
        }

        private JsonResult LiquidJson(object obj)
        {
            return new JsonResult(obj, new JsonSerializerSettings
            {
                //ContractResolver = new LiquidThemeEngine.Filters.RubyContractResolver(),
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
