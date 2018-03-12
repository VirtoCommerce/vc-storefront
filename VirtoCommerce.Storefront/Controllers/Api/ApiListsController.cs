using Microsoft.AspNetCore.Mvc;
using PagedList.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Lists;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiListsController : StorefrontControllerBase
    {
        private readonly IWishlistBuilder _wishlistBuilder;
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;
        private readonly ICartService _cartService;

        public ApiListsController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, IWishlistBuilder listBuilder, ICartBuilder cartBuilder, IStorefrontUrlBuilder urlBuilder, ICartService cartService)
            : base(workContextAccessor, urlBuilder)
        {
            _wishlistBuilder = listBuilder;
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
            _cartService = cartService;
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

        // POST: storefrontapi/lists/getlistswithproduct
        [HttpPost]
        public async Task<ActionResult> GetListsWithProduct([FromBody] GetListsWithProductRequest request)
        {
            var result = new List<string>();
            //Need lock to prevent concurrent access to same cart
            foreach (var listName in request.ListNames)
            {
                using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, listName)).LockAsync())
                {
                    var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
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

        // POST: storefrontapi/lists/search
        [HttpPost]
        public async Task<ActionResult> SearchLists([FromBody] ShoppingCartSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new ShoppingCartSearchCriteria();
            }

            //restricting query to lists search, only paging and sorting are available
            searchCriteria.Type = "wishlist";
            searchCriteria.StoreId = WorkContext.CurrentStore.Id;
            searchCriteria.Customer = WorkContext.CurrentUser;
            searchCriteria.Currency = WorkContext.CurrentCurrency;
            searchCriteria.Language = WorkContext.CurrentLanguage;

            var cartPagedList = await _cartService.SearchShoppingCartsAsync(searchCriteria);

            ////todo: move "default" cart filtering to module api
            //var currentCartId = WorkContext.CurrentCart.Value?.Id;
            //if (!currentCartId.IsNullOrEmpty())
            //{
            //    var filteredCarts = cartPagedList.Where(x => x.Id != currentCartId).ToList();
            //    cartPagedList = new StaticPagedList<ShoppingCart>(filteredCarts, searchCriteria.PageNumber, searchCriteria.PageSize, cartPagedList.TotalItemCount);
            //}

            return Json(new
            {
                Results = cartPagedList,
                TotalCount = cartPagedList.TotalItemCount
            });
        }

        // POST: storefrontapi/lists/{listName}/create
        [HttpPost]
        public async Task<ActionResult> CreateList(string listName)
        {
            var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
            if (wishlistBuilder.Cart.IsTransient())
            {
                await wishlistBuilder.SaveAsync();
            }
            return Json(wishlistBuilder.Cart);
        }

        // DELETE: storefrontapi/lists/deletelistsbyids?listIds=...&listIds=...
        [HttpDelete]
        public async Task<ActionResult> DeleteListsByIds(string[] listIds)
        {
            await _cartService.DeleteCartsByIdsAsync(listIds);
            return Ok();
        }

        // POST: storefrontapi/lists/{listName}/mergewithcurrentcart
        [HttpPost]
        public async Task<ActionResult> MergeWithCurrentCart(string listName)
        {
            var currentCartName = WorkContext.CurrentCart.Value?.Name;
            using (await AsyncLock.GetLockByKey(GetAsyncLockCartKey(WorkContext, currentCartName)).LockAsync())
            {
                //load list
                var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
                var list = wishlistBuilder.Cart;

                //load or create default cart
                var cartBuilder = await LoadOrCreateDefaultCartAsync(currentCartName);
                foreach(var lineItem in list.Items)
                {
                    await cartBuilder.AddItemAsync(lineItem.Product, 1);
                }

                await cartBuilder.SaveAsync();
                return Ok();
            }
        }

        private static string GetAsyncLockCartKey(WorkContext context, string listName)
        {
            return string.Join(":", listName, "wishlist", context.CurrentUser.Id, context.CurrentStore.Id);
        }

        private async Task<ICartBuilder> LoadOrCreateWishlistAsync(string listName)
        {
            await _wishlistBuilder.LoadOrCreateNewTransientCartAsync(listName, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);
            return _wishlistBuilder;
        }

        private async Task<ICartBuilder> LoadOrCreateDefaultCartAsync(string currentCartName)
        {
            await _cartBuilder.LoadOrCreateNewTransientCartAsync(currentCartName, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency);
            return _cartBuilder;
        }
    }
}
