using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
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
        private readonly ICartService _cartService;

        public ApiListsController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, ICartBuilder cartBuilder, IStorefrontUrlBuilder urlBuilder, ICartService cartService)
            : base(workContextAccessor, urlBuilder)
        {
            _wishlistBuilder = cartBuilder;
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

        [HttpPost]
        public async Task<ActionResult> SearchLists([FromForm] ShoppingCartSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new ShoppingCartSearchCriteria();
            }

            //restricting ajax query to lists search, only paging and sorting are available
            searchCriteria.StoreId = WorkContext.CurrentStore.Id;
            searchCriteria.Customer = WorkContext.CurrentUser;
            searchCriteria.Currency = WorkContext.CurrentCurrency;
            searchCriteria.Language = WorkContext.CurrentLanguage;

            var cartPagedList = await _cartService.SearchShoppingCartsAsync(searchCriteria);

            //todo: move "default" cart filtering to module api
            var currentCartId = WorkContext.CurrentCart.Value?.Id;
            if (!currentCartId.IsNullOrEmpty())
            {
                var filteredCarts = cartPagedList.Where(x => x.Id != currentCartId).ToList();
                cartPagedList = new StaticPagedList<ShoppingCart>(filteredCarts, searchCriteria.PageNumber, searchCriteria.PageSize, cartPagedList.TotalItemCount);
            }
            return Json(cartPagedList);
        }

        [HttpPost]
        public async Task<ActionResult> CreateList(string listName)
        {
            var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
            await wishlistBuilder.SaveAsync();
            return Json(wishlistBuilder.Cart);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePredefindedLists([FromBody] string[] listNames)
        {
            foreach(var listName in listNames)
            {
                var searchCriteria = new ShoppingCartSearchCriteria()
                {
                    Name = listName,
                    StoreId = WorkContext.CurrentStore.Id,
                    Customer = WorkContext.CurrentUser,
                    Currency = WorkContext.CurrentCurrency,
                    Language = WorkContext.CurrentLanguage,
                };
                var searcResults = await _cartService.SearchShoppingCartsAsync(searchCriteria);
                if (!searcResults.Any())
                {
                    var wishlistBuilder = await LoadOrCreateWishlistAsync(listName);
                    await wishlistBuilder.SaveAsync();
                }
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteListsByIds(string[] listIds)
        {
            await _cartService.DeleteCartsByIdsAsync(listIds);
            return Ok();
        }

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
                wishlistBuilder = await LoadOrCreateWishlistAsync(currentCartName);
                foreach(var lineItem in list.Items)
                {
                    await wishlistBuilder.AddItemAsync(lineItem.Product, 1);
                }

                await wishlistBuilder.SaveAsync();

                return Ok();
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
