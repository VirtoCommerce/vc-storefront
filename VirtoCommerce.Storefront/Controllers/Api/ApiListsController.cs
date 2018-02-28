using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Lists;
using VirtoCommerce.Storefront.Model.Services;
using cartModel = VirtoCommerce.Storefront.AutoRestClients.CartModuleApi.Models;


namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiListsController : StorefrontControllerBase
    {
        private readonly ICartBuilder _wishlistBuilder;
        private readonly ICatalogService _catalogService;

        public ApiListsController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, ICartBuilder cartBuilder,
                                     IStorefrontUrlBuilder urlBuilder)
            : base(workContextAccessor, urlBuilder)
        {
            _wishlistBuilder = cartBuilder;
            _catalogService = catalogService;
        }

        // GET: storefrontapi/lists
        [HttpGet]
        public async Task<ActionResult> GetListByName(string listName)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName)).LockAsync())
            {
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
                await wishlistBuilder.ValidateAsync();
                return Json(wishlistBuilder.Cart);
            }
        }

        // GET: storefrontapi/lists/contains
        [HttpGet]
        public async Task<ActionResult> IsItemContainsInList(string productId, string listName)
        {
            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName)).LockAsync())
            {
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
                await wishlistBuilder.ValidateAsync();
                var hasProduct = wishlistBuilder.Cart.Items.Any(x => x.ProductId == productId);
                return Json(new { Contains = hasProduct });
            }
        }

        // POST: storefrontapi/lists/filterlistsbyproduct
        [HttpPost]
        public async Task<ActionResult> FilterListsByProduct([FromBody] FilterListsByProductRequest request)
        {
            var result = new List<string>();
            //Need lock to prevent concurrent access to same cart
            foreach(var listName in request.ListNames)
            {
                using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName)).LockAsync())
                {
                    var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
                    await wishlistBuilder.ValidateAsync();
                    var hasProduct = wishlistBuilder.Cart.Items.Any(x => x.ProductId == request.ProdcutId);
                    if (hasProduct)
                    {
                        result.Add(listName);
                    }
                }
            }
            return Json(result);
        }

        // POST: storefrontapi/lists/items
        [HttpPost]
        public async Task<ActionResult> AddItemToList([FromBody] AddListItem listItem)
        {
            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listItem.ListName)).LockAsync())
            {
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listItem.ListName);

                var products = await _catalogService.GetProductsAsync(new[] { listItem.ProductId }, Model.Catalog.ItemResponseGroup.Inventory | Model.Catalog.ItemResponseGroup.ItemWithPrices);
                if (products != null && products.Any())
                {
                    await wishlistBuilder.AddItemAsync(products.First(), 1);
                    await wishlistBuilder.SaveAsync();
                }
                return Json(new { ItemsCount = wishlistBuilder.Cart.ItemsQuantity });
            }
        }

        // DELETE: storefrontapi/lists/items?id=...
        [HttpDelete]
        public async Task<ActionResult> RemoveItemFromList(string lineItemId, string listName)
        {
            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName)).LockAsync())
            {
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
                await wishlistBuilder.RemoveItemAsync(lineItemId);
                await wishlistBuilder.SaveAsync();
                return Json(new { ItemsCount = wishlistBuilder.Cart.ItemsQuantity });
            }
        }

        private static string GetAsyncLockCartKey(WorkContext context, string listName)
        {
            return string.Join(":", listName, context.CurrentUser.Id, context.CurrentStore.Id);
        }

         private async Task<ICartBuilder> LoadOrCreateWishlistAsync(string listName)
        {
            await _wishlistBuilder.LoadOrCreateNewTransientCartAsync(listName, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);
            return _wishlistBuilder;
        }
    }
}
