using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Domain.Lists;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Lists;
using VirtoCommerce.Storefront.Model.Lists.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiListsController : StorefrontControllerBase
    {
        private readonly IWishlistBuilder _wishlistBuilder;
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;
        private readonly IWishlistService _listService;
        private readonly StorefrontOptions _options;

        public ApiListsController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, IWishlistBuilder listBuilder, ICartBuilder cartBuilder, IStorefrontUrlBuilder urlBuilder, IWishlistService listService, IOptions<StorefrontOptions> options)
            : base(workContextAccessor, urlBuilder)
        {
            _wishlistBuilder = listBuilder;
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
            _listService = listService;
            _options = options.Value;
        }

        // GET: storefrontapi/lists/{listName}/{type}
        [HttpGet]
        public async Task<ActionResult> GetListByName(string listName, string type)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName, type)).LockAsync())
            {
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listName, type);
                return Json(wishlistBuilder.Cart);
            }
        }

        // POST: storefrontapi/lists/getlistswithproduct
        [HttpPost]
        public async Task<ActionResult> GetListsWithProduct([FromBody] GetListsWithProductRequest request)
        {
            var result = new List<string>();
            //Need lock to prevent concurrent access to same list
            foreach (var listName in request.ListNames)
            {
                using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName, request.Type)).LockAsync())
                {
                    var wishlistBuilder = await LoadOrCreateWishlistAsync(listName, request.Type);
                    await wishlistBuilder.ValidateAsync();
                    var hasProduct = wishlistBuilder.Cart.Items.Any(x => x.ProductId == request.ProductId);
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
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listItem.ListName, listItem.Type)).LockAsync())
            {
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listItem.ListName, listItem.Type);

                var products = await _catalogService.GetProductsAsync(new[] { listItem.ProductId }, Model.Catalog.ItemResponseGroup.Inventory | Model.Catalog.ItemResponseGroup.ItemWithPrices);
                if (products != null && products.Any())
                {
                    await wishlistBuilder.AddItemAsync(products.First(), 1);
                    await wishlistBuilder.SaveAsync();
                }
                return Json(new { ItemsCount = wishlistBuilder.Cart.ItemsQuantity });
            }
        }

        // DELETE: storefrontapi/lists/{listName}/type/items/{lineItemId}
        [HttpDelete]
        public async Task<ActionResult> RemoveItemFromList(string lineItemId, string listName, string type)
        {
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName, type)).LockAsync())
            {
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listName, type);
                await wishlistBuilder.RemoveItemAsync(lineItemId);
                await wishlistBuilder.SaveAsync();
                return Json(new { ItemsCount = wishlistBuilder.Cart.ItemsQuantity });
            }
        }

        // POST: storefrontapi/lists/search
        [HttpPost]
        public async Task<ActionResult> SearchLists([FromBody] WishlistSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new WishlistSearchCriteria();
            }

            //restricting query to lists belongs to other customers
            searchCriteria.StoreId = WorkContext.CurrentStore.Id;
            searchCriteria.Customer = WorkContext.CurrentUser;
            searchCriteria.Currency = WorkContext.CurrentCurrency;
            searchCriteria.Language = WorkContext.CurrentLanguage;

            var cartPagedList = await _listService.SearchWishlistsAsync(searchCriteria);

            return Json(new
            {
                Results = cartPagedList,
                TotalCount = cartPagedList.TotalItemCount
            });
        }

        // POST: storefrontapi/lists/{listName}/{type}/create
        [HttpPost]
        public async Task<ActionResult> CreateList(string listName, string type)
        {
            var totalCount = await _listService.GetWishlistCountByCustomer(WorkContext.CurrentUser);
            if (totalCount >= _options.WishlistLimit)
            {
                return Json(new
                {
                    Error = "Wishlists count exceeds limit"
                });
            }

            var list = (await LoadOrCreateWishlistAsync(listName, type)).Cart;
            if (list.IsTransient())
            {
                list = await _listService.CreateListAsync(list.ToWishlist(WorkContext.CurrentCurrency, WorkContext.CurrentLanguage, WorkContext.CurrentUser));
            }

            return Json(list);
        }

        // DELETE: storefrontapi/lists/deletelistsbyids?listIds=...&listIds=...
        [HttpDelete]
        public async Task<ActionResult> DeleteListsByIds(string[] listIds)
        {
            await _listService.DeleteListsByIdsAsync(listIds);
            return Ok();
        }

        // POST: storefrontapi/lists/{listName}/{type}/mergewithcurrentcart
        [HttpPost]
        public async Task<ActionResult> MergeWithCurrentCart(string listName, string type)
        {
            var currentCartName = WorkContext.CurrentCart.Value?.Name;
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, currentCartName, string.Empty)).LockAsync())
            {
                //load list
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listName, type);
                var list = wishlistBuilder.Cart;

                //load or create default cart
                var cartBuilder = await LoadOrCreateDefaultCartAsync(currentCartName);
                await cartBuilder.MergeWithCartAsync(list);

                await cartBuilder.SaveAsync();
                return Ok();
            }
        }

        private static string GetAsyncLockCartKey(WorkContext context, string listName, string type)
        {
            return string.Join(":", listName, context.CurrentUser.Id, context.CurrentStore.Id, type);
        }

        private async Task<ICartBuilder> LoadOrCreateWishlistAsync(string listName, string type)
        {
            await _wishlistBuilder.LoadOrCreateNewTransientCartAsync(listName, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency, type);
            return _wishlistBuilder;
        }

        private async Task<ICartBuilder> LoadOrCreateDefaultCartAsync(string currentCartName)
        {
            await _cartBuilder.LoadOrCreateNewTransientCartAsync(currentCartName, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);
            return _cartBuilder;
        }
    }
}
