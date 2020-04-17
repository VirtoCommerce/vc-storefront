using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Events;
using VirtoCommerce.Storefront.Model.Order.Events;
using VirtoCommerce.Storefront.Model.Services;
using orderModel = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("lists")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiListsController : StorefrontControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartBuilder _cartBuilder;
        private readonly IOrderModule _orderApi;
        private readonly IEventPublisher _publisher;
        private readonly ICatalogService _catalogService;

        public ApiListsController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, ICartService cartService, ICartBuilder cartBuilder, IStorefrontUrlBuilder urlBuilder, IOrderModule orderApi, IEventPublisher publisher)
            : base(workContextAccessor, urlBuilder)
        {
            _cartBuilder = cartBuilder;
            _catalogService = catalogService;
            _cartService = cartService;
            _publisher = publisher;
            _orderApi = orderApi;
        }

        // GET: storefrontapi/lists/{listName}/{type}
        [HttpGet("{listName}/{type}")]
        public async Task<ActionResult<ShoppingCart>> GetListByName([FromRoute]string listName, [FromRoute]string type)
        {
            var unescapedListName = Uri.UnescapeDataString(listName);
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, unescapedListName, type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(unescapedListName, type);
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
            var unescapedListName = Uri.UnescapeDataString(listName);
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, unescapedListName, type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(unescapedListName, type);
                await cartBuilder.RemoveItemAsync(lineItemId);
                await cartBuilder.SaveAsync();
                return new ShoppingCartItems { ItemsCount = cartBuilder.Cart.ItemsQuantity };
            }
        }

        // PUT: storefrontapi/lists/{listName}/{type}/items
        [HttpPut("{listName}/{type}/items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeListItem(string listName, string type, [FromBody] ChangeCartItemQty changeQty)
        {
            var unescapedListName = Uri.UnescapeDataString(listName);
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, unescapedListName, type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(unescapedListName, type);

                var lineItem = cartBuilder.Cart.Items.FirstOrDefault(i => i.Id == changeQty.LineItemId);
                if (lineItem != null)
                {
                    await cartBuilder.ChangeItemQuantityAsync(changeQty.LineItemId, changeQty.Quantity);
                    await cartBuilder.SaveAsync();
                }
            }
            return Ok();
        }

        // POST: storefrontapi/lists/{listName}/{type}/clear
        [HttpPost("{listName}/{type}/clear")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClearList(string listName, string type)
        {
            var unescapedListName = Uri.UnescapeDataString(listName);
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, unescapedListName, type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(unescapedListName, type);
                await cartBuilder.ClearAsync();
                await cartBuilder.SaveAsync();
            }
            return Ok();
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
            var cartBuilder = await LoadOrCreateCartAsync(Uri.UnescapeDataString(listName), type);
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
                var cartBuilder = await LoadOrCreateCartAsync(Uri.UnescapeDataString(listName), type);
                var listCart = cartBuilder.Cart;

                //load or create default cart
                cartBuilder = await LoadOrCreateCartAsync(currentCartName);
                await cartBuilder.MergeWithCartAsync(listCart);

                await cartBuilder.SaveAsync();
                return Ok();
            }
        }

        // POST: storefrontapi/lists/{listName}/{type}/createorder
        [HttpPost("{listName}/{type}/createorder")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<OrderCreatedInfo>> CreateOrderFromList(string listName, string type, [FromBody] BankCardInfo bankCardInfo)
        {
            var unescapedListName = Uri.UnescapeDataString(listName);
            //Need lock to prevent concurrent access to same list
            using (await AsyncLock.GetLockByKey(GetAsyncListKey(WorkContext, unescapedListName, type)).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync(unescapedListName, type);

                var orderDto = await _orderApi.CreateOrderFromCartAsync(cartBuilder.Cart.Id);
                var order = orderDto.ToCustomerOrder(WorkContext.AllCurrencies, WorkContext.CurrentLanguage);

                var taskList = new List<Task>
                {
                    //Raise domain event asynchronously
                    _publisher.Publish(new OrderPlacedEvent(WorkContext, orderDto.ToCustomerOrder(WorkContext.AllCurrencies, WorkContext.CurrentLanguage), cartBuilder.Cart)),
                    //Remove the cart asynchronously
                    cartBuilder.RemoveCartAsync()
                };
                //Process order asynchronously
                var incomingPaymentDto = orderDto.InPayments?.FirstOrDefault();
                Task<orderModel.ProcessPaymentResult> processPaymentTask = null;
                if (incomingPaymentDto != null)
                {
                    processPaymentTask = _orderApi.ProcessOrderPaymentsAsync(orderDto.Id, incomingPaymentDto.Id, bankCardInfo.ToBankCardInfoDto());
                    taskList.Add(processPaymentTask);
                }
                await Task.WhenAll(taskList.ToArray());

                return new OrderCreatedInfo
                {
                    Order = order,
                    OrderProcessingResult = processPaymentTask != null ? (await processPaymentTask).ToProcessPaymentResult(order) : null,
                    PaymentMethod = incomingPaymentDto?.PaymentMethod.ToPaymentMethod(order),
                };
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
