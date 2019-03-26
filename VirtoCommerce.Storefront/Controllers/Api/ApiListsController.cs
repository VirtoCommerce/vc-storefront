using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.SearchResults;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("lists")]
    public class ApiListsController : StorefrontControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartBuilder _cartBuilder;
        private readonly ICatalogService _catalogService;

        public ApiListsController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, ICartService cartService, ICartBuilder cartBuilder, IStorefrontUrlBuilder urlBuilder)
            : base(workContextAccessor, urlBuilder)
        {
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
            _cartService = cartService;
        }

        // GET: storefrontapi/lists/{listName}/{type}
        [HttpGet("{listName}/{type}")]
        public async Task<ActionResult<ShoppingCart>> GetListByName([FromRoute]string listName, [FromRoute]string type)
        {
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, listName, type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(listName, type);
                return cartBuilder.Cart;
            }
        }

        // POST: storefrontapi/lists/getlistswithproduct
        [HttpPost("getlistswithproduct")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<List<string>>> GetListsWithProduct([FromBody] GetCartsWithProductRequest request)
        {
            var result = new List<string>();
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, "*", request.Type)).LockAsync())
            {
                var criteria = new CartSearchCriteria
                {
                    Type = request.Type,
                    StoreId = WorkContext.CurrentStore.Id,
                    Customer = WorkContext.CurrentUser,
                    Currency = WorkContext.CurrentCurrency,
                    Language = WorkContext.CurrentLanguage,
                    PageSize = int.MaxValue
                };
                var carts = await _cartService.SearchCartsAsync(criteria);
                result.AddRange(carts.Where(c => c.Items.Any(i => i.ProductId == request.ProductId)).Select(x => x.Name));
            }
            return result;
        }

        // POST: storefrontapi/lists/items
        [HttpPost("items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ShoppingCartItems>> AddItemToList([FromBody] AddCartItem listItem)
        {
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, listItem.ListName, listItem.Type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(listItem.ListName, listItem.Type);

                var products = await _catalogService.GetProductsAsync(new[] { listItem.ProductId }, Model.Catalog.ItemResponseGroup.Inventory | Model.Catalog.ItemResponseGroup.ItemWithPrices);
                if (products != null && products.Any())
                {
                    await cartBuilder.AddItemAsync(products.First(), 1);
                    await cartBuilder.SaveAsync();
                }
                return new ShoppingCartItems { ItemsCount = cartBuilder.Cart.ItemsQuantity };
            }
        }

        // DELETE: storefrontapi/lists/{listName}/type/items/{lineItemId}
        [HttpDelete("{listName}/{type}/items/{lineItemId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ShoppingCartItems>> RemoveItemFromList(string lineItemId, string listName, string type)
        {
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, listName, type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(listName, type);
                await cartBuilder.RemoveItemAsync(lineItemId);
                await cartBuilder.SaveAsync();
                return new ShoppingCartItems { ItemsCount = cartBuilder.Cart.ItemsQuantity };
            }
        }

        // POST: storefrontapi/lists/search
        [HttpPost("search")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ShoppingCartSearchResult>> SearchLists([FromBody] CartSearchCriteria searchCriteria)
        {
            if (searchCriteria == null)
            {
                searchCriteria = new CartSearchCriteria();
            }

            //restricting query to lists belongs to other customers
            searchCriteria.StoreId = WorkContext.CurrentStore.Id;
            searchCriteria.Customer = WorkContext.CurrentUser;
            searchCriteria.Currency = WorkContext.CurrentCurrency;
            searchCriteria.Language = WorkContext.CurrentLanguage;

            var cartPagedList = await _cartService.SearchCartsAsync(searchCriteria);

            return new ShoppingCartSearchResult
            {
                Results = cartPagedList.ToArray(),
                TotalCount = cartPagedList.TotalItemCount
            };
        }

        // POST: storefrontapi/lists/{listName}/{type}/create
        [HttpPost("{listName}/{type}/create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ShoppingCart>> CreateList(string listName, string type)
        {
            var cartBuilder = await LoadOrCreateCartAsync(listName, type);
            if (cartBuilder.Cart.IsTransient())
            {
                await cartBuilder.SaveAsync();
            }
            return cartBuilder.Cart;
        }

        // DELETE: storefrontapi/lists/deletelistsbyids?listIds=...&listIds=...
        [HttpDelete("deletelistsbyids")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteListsByIds(string[] listIds)
        {
            //filter out the lists that don't belong to the current user
            foreach (var id in listIds)
            {
                var cart = await _cartService.GetByIdAsync(id);
                if (cart != null && cart.CustomerId == WorkContext.CurrentUser.Id)
                {
                    await _cartBuilder.TakeCartAsync(cart);
                    await _cartBuilder.RemoveCartAsync();
                }
            }
            return Ok();
        }

        // POST: storefrontapi/lists/{listName}/{type}/mergewithcurrentcart
        [HttpPost("{listName}/{type}/mergewithcurrentcart")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MergeWithCurrentCart(string listName, string type)
        {
            var currentCartName = WorkContext.CurrentCart.Value?.Name;
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, currentCartName, string.Empty)).LockAsync())
            {
                //load list
                var cartBuilder = await LoadOrCreateCartAsync(listName, type);
                var listCart = cartBuilder.Cart;

                //load or create default cart
                cartBuilder = await LoadOrCreateCartAsync(currentCartName);
                await cartBuilder.MergeWithCartAsync(listCart);

                await cartBuilder.SaveAsync();
                return Ok();
            }
        }

        private static string GetAsyncListKey(WorkContext context, string listName, string type)
        {
            return string.Join(":", listName, context.CurrentUser.Id, context.CurrentStore.Id, type);
        }

        private async Task<ICartBuilder> LoadOrCreateCartAsync(string cartName, string type = null)
        {
            await _cartBuilder.LoadOrCreateNewTransientCartAsync(cartName, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency, type);
            return _cartBuilder;
        }


    }
}
