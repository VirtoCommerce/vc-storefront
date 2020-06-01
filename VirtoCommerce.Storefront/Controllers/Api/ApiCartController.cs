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
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Order.Events;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using orderModel = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("cart")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiCartController : StorefrontControllerBase
    {
        private readonly ICartBuilder _cartBuilder;
        private readonly IOrderModule _orderApi;
        private readonly ICatalogService _catalogService;
        private readonly IEventPublisher _publisher;
        private readonly ISubscriptionService _subscriptionService;
        public ApiCartController(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, ICartBuilder cartBuilder,
                                 IOrderModule orderApi, IStorefrontUrlBuilder urlBuilder,
                                 IEventPublisher publisher, ISubscriptionService subscriptionService)
            : base(workContextAccessor, urlBuilder)
        {
            _cartBuilder = cartBuilder;
            _orderApi = orderApi;
            _catalogService = catalogService;
            _publisher = publisher;
            _subscriptionService = subscriptionService;
        }

        // Get current user shopping cart
        // GET: storefrontapi/cart
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCart()
        {
            var cartBuilder = await LoadOrCreateCartAsync();
            await cartBuilder.ValidateAsync();
            return cartBuilder.Cart;
        }

        // PUT: storefrontapi/cart/comment
        [HttpPut("comment")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCartComment([FromBody] UpdateCartCommentRequest commentRequest)
        {
            EnsureCartExists();

            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                var comment = commentRequest?.Comment;

                await cartBuilder.UpdateCartComment(comment);
                await cartBuilder.SaveAsync();
            }

            return Ok();
        }

        // GET: storefrontapi/cart/itemscount
        [HttpGet("itemscount")]
        public async Task<ActionResult<int>> GetCartItemsCount()
        {
            EnsureCartExists();

            var cartBuilder = await LoadOrCreateCartAsync();

            return cartBuilder.Cart.ItemsQuantity;
        }

        // POST: storefrontapi/cart/items
        [HttpPost("items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ShoppingCartItems>> AddItemToCart([FromBody] AddCartItem cartItem)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();

                var products = await _catalogService.GetProductsAsync(new[] { cartItem.ProductId }, Model.Catalog.ItemResponseGroup.Inventory | Model.Catalog.ItemResponseGroup.ItemWithPrices);
                if (products != null && products.Any())
                {
                    cartItem.Product = products.First();
                    if (await cartBuilder.AddItemAsync(cartItem))
                    {
                        await cartBuilder.SaveAsync();
                    }
                }
                return new ShoppingCartItems { ItemsCount = cartBuilder.Cart.ItemsQuantity };
            }
        }

        // PUT: storefrontapi/cart/items/price
        [HttpPut("items/price")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeCartItemPrice([FromBody] ChangeCartItemPrice newPrice)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ChangeItemPriceAsync(newPrice);

                await cartBuilder.SaveAsync();

            }
            return Ok();
        }

        // PUT: storefrontapi/cart/items?lineItemId=...&quantity=...
        [HttpPut("items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeCartItem([FromBody] ChangeCartItemQty changeQty)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();

                await cartBuilder.ChangeItemQuantityAsync(changeQty);
                await cartBuilder.SaveAsync();
            }
            return Ok();
        }

        // DELETE: storefrontapi/cart/items?lineItemId=...
        [HttpDelete("items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ShoppingCartItems>> RemoveCartItem(string lineItemId)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.RemoveItemAsync(lineItemId);
                await cartBuilder.SaveAsync();
                return new ShoppingCartItems { ItemsCount = cartBuilder.Cart.ItemsQuantity };
            }

        }

        // POST: storefrontapi/cart/clear
        [HttpPost("clear")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClearCart()
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.ClearAsync();
                await cartBuilder.SaveAsync();
            }
            return Ok();
        }

        // GET: storefrontapi/cart/shipments/{shipmentId}/shippingmethods
        [HttpGet("shipments/{shipmentId}/shippingmethods")]
        public async Task<ActionResult<IEnumerable<ShippingMethod>>> GetCartShipmentAvailShippingMethods(string shipmentId)
        {
            EnsureCartExists();

            var cartBuilder = await LoadOrCreateCartAsync();

            var shippingMethods = await cartBuilder.GetAvailableShippingMethodsAsync();
            return shippingMethods.ToList();
        }

        // GET: storefrontapi/cart/paymentmethods
        [HttpGet("paymentmethods")]
        public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetCartAvailPaymentMethods()
        {
            EnsureCartExists();

            var cartBuilder = await LoadOrCreateCartAsync();

            var paymentMethods = await cartBuilder.GetAvailablePaymentMethodsAsync();
            return paymentMethods.ToList();
        }

        // POST: storefrontapi/cart/coupons/{couponCode}
        [HttpPost("coupons/{couponCode}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCartCoupon(string couponCode)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();

                await cartBuilder.AddCouponAsync(couponCode);

                await cartBuilder.SaveAsync();

                return Ok();
            }
        }

        // POST: storefrontapi/cart/coupons/validate
        [HttpPost("coupons/validate")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<Coupon>> ValidateCoupon([FromBody]Coupon coupon)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                await _cartBuilder.TakeCartAsync(WorkContext.CurrentCart.Value.Clone() as ShoppingCart);
                _cartBuilder.Cart.Coupons = new[] { coupon };
                await _cartBuilder.EvaluatePromotionsAsync();
                return Ok(coupon);
            }
        }


        // DELETE: storefrontapi/cart/coupons
        [HttpDelete("coupons")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveCartCoupon([FromQuery]string couponCode = null)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.RemoveCouponAsync(couponCode);
                await cartBuilder.SaveAsync();
            }

            return Ok();
        }


        // POST: storefrontapi/cart/paymentPlan    
        [HttpPost("paymentPlan")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdateCartPaymentPlan([FromBody] PaymentPlan paymentPlan)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                paymentPlan.Id = cartBuilder.Cart.Id;

                await _subscriptionService.UpdatePaymentPlanAsync(paymentPlan);
                // await _cartBuilder.SaveAsync();
                cartBuilder.Cart.PaymentPlan = paymentPlan;
            }
            return Ok();
        }

        // DELETE: storefrontapi/cart/paymentPlan    
        [HttpDelete("paymentPlan")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCartPaymentPlan()
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await _subscriptionService.DeletePlansByIdsAsync(new[] { cartBuilder.Cart.Id });
                // await _cartBuilder.SaveAsync();
                cartBuilder.Cart.PaymentPlan = null;
            }
            return Ok();
        }


        // POST: storefrontapi/cart/shipments    
        [HttpPost("shipments")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdateCartShipment([FromBody] Shipment shipment)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.AddOrUpdateShipmentAsync(shipment);
                await cartBuilder.SaveAsync();
            }

            return Ok();
        }

        // POST: storefrontapi/cart/payments
        [HttpPost("payments")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrUpdateCartPayment([FromBody] Payment payment)
        {
            EnsureCartExists();

            if (payment.Amount.Amount == decimal.Zero)
            {
                return BadRequest("Valid payment amount is required");
            }

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.AddOrUpdatePaymentAsync(payment);
                await cartBuilder.SaveAsync();
            }
            return Ok();
        }

        // POST: storefrontapi/cart/createorder
        [HttpPost("createorder")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<OrderCreatedInfo>> CreateOrderFromDefaultCart([FromBody] BankCardInfo bankCardInfo)
        {
            var cartBuilder = await LoadOrCreateCartAsync();
            if (cartBuilder.Cart.IsTransient())
            {
                return BadRequest($"The default cart doesn't exist");
            }
            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(cartBuilder.Cart.GetCacheKey()).LockAsync())
            {
                return await CreateOrderFromCartAsync(cartBuilder, bankCardInfo, removeCart: true);
            }
        }

        // POST: storefrontapi/cart/{name}/{type}/createorder?removeCart=true
        [HttpPost("{name}/{type}/createorder")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<OrderCreatedInfo>> CreateOrderFromNamedCart([FromRoute] string name, [FromRoute] string type, [FromBody] BankCardInfo bankCardInfo, [FromQuery]bool removeCart)
        {
            var cartBuilder = await LoadOrCreateCartAsync(Uri.UnescapeDataString(name), type);
            if (cartBuilder.Cart.IsTransient())
            {
                return BadRequest($"The cart with name: {name} and type: {type} doesn't exist");
            }
            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(cartBuilder.Cart.GetCacheKey()).LockAsync())
            {
                return await CreateOrderFromCartAsync(cartBuilder, bankCardInfo, removeCart);
            }
        }

        private async Task<OrderCreatedInfo> CreateOrderFromCartAsync(ICartBuilder cartBuilder, BankCardInfo bankCardInfo, bool removeCart)
        {
            var orderDto = await _orderApi.CreateOrderFromCartAsync(cartBuilder.Cart.Id);
            var order = orderDto.ToCustomerOrder(WorkContext.AllCurrencies, WorkContext.CurrentLanguage);

            var taskList = new List<Task>
                {
                    //Raise domain event asynchronously
                    _publisher.Publish(new OrderPlacedEvent(WorkContext, orderDto.ToCustomerOrder(WorkContext.AllCurrencies, WorkContext.CurrentLanguage), cartBuilder.Cart)),
                    //Remove the cart asynchronously
                    removeCart ? cartBuilder.RemoveCartAsync() : Task.CompletedTask
                };
                //Process order asynchronously
                var incomingPaymentDto = orderDto.InPayments?.FirstOrDefault();
                Task<orderModel.ProcessPaymentResult> processPaymentTask = null;
                if (incomingPaymentDto != null)
                {
                    processPaymentTask = _orderApi.ProcessOrderPaymentsAsync(orderDto.Id, incomingPaymentDto.Id, bankCardInfo?.ToBankCardInfoDto());
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

        private void EnsureCartExists()
        {
            if (WorkContext.CurrentCart.Value == null)
            {
                throw new StorefrontException("Cart not found");
            }
        }

        private async Task<ICartBuilder> LoadOrCreateCartAsync(string cartName = null, string type = null)
        {
            //Need to try load fresh cart from cache or service to prevent parallel update conflict
            //because WorkContext.CurrentCart may contains old cart
            await _cartBuilder.LoadOrCreateNewTransientCartAsync(cartName ?? WorkContext.CurrentCart.Value.Name, WorkContext.CurrentStore, WorkContext.CurrentUser, WorkContext.CurrentLanguage, WorkContext.CurrentCurrency, type);
            return _cartBuilder;
        }
    }
}
