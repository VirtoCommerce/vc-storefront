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
using VirtoCommerce.Storefront.Model.Order.Events;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Subscriptions;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using orderModel = VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("cart")]
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
        public async Task<ActionResult> GetCart()
        {
            var cartBuilder = await LoadOrCreateCartAsync();
            await cartBuilder.ValidateAsync();
            return Json(cartBuilder.Cart);
        }

        // PUT: storefrontapi/cart/comment
        [HttpPut("comment")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCartComment([FromBody] string comment)
        {
            EnsureCartExists();

            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();

                await cartBuilder.UpdateCartComment(comment);
                await cartBuilder.SaveAsync();
            }

            return Ok();
        }

        // GET: storefrontapi/cart/itemscount
        [HttpGet("itemscount")]
        public async Task<ActionResult> GetCartItemsCount()
        {
            EnsureCartExists();

            var cartBuilder = await LoadOrCreateCartAsync();

            return Json(cartBuilder.Cart.ItemsQuantity);
        }

        // POST: storefrontapi/cart/items
        [HttpPost("items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItemToCart([FromBody] AddCartItem cartItem)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();

                var products = await _catalogService.GetProductsAsync(new[] { cartItem.ProductId }, Model.Catalog.ItemResponseGroup.Inventory | Model.Catalog.ItemResponseGroup.ItemWithPrices);
                if (products != null && products.Any())
                {
                    await cartBuilder.AddItemAsync(products.First(), cartItem.Quantity);
                    await cartBuilder.SaveAsync();
                }
                return Json(new { ItemsCount = cartBuilder.Cart.ItemsQuantity });
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

                var lineItem = cartBuilder.Cart.Items.FirstOrDefault(x => x.Id == newPrice.LineItemId);
                if (lineItem != null)
                {
                    var newPriceMoney = new Money(newPrice.NewPrice, cartBuilder.Cart.Currency);
                    //do not allow to set less price via this API call
                    if (lineItem.ListPrice < newPriceMoney)
                    {
                        lineItem.ListPrice = newPriceMoney;
                    }
                    if (lineItem.SalePrice < newPriceMoney)
                    {
                        lineItem.SalePrice = newPriceMoney;
                    }
                }
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

                var lineItem = cartBuilder.Cart.Items.FirstOrDefault(i => i.Id == changeQty.LineItemId);
                if (lineItem != null)
                {
                    await cartBuilder.ChangeItemQuantityAsync(changeQty.LineItemId, changeQty.Quantity);
                    await cartBuilder.SaveAsync();
                }
            }
            return Ok();
        }

        // DELETE: storefrontapi/cart/items?lineItemId=...
        [HttpDelete("items")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveCartItem(string lineItemId)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.RemoveItemAsync(lineItemId);
                await cartBuilder.SaveAsync();
                return Json(new { ItemsCount = cartBuilder.Cart.ItemsQuantity });
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
        public async Task<ActionResult> GetCartShipmentAvailShippingMethods(string shipmentId)
        {
            EnsureCartExists();

            var cartBuilder = await LoadOrCreateCartAsync();

            var shippingMethods = await cartBuilder.GetAvailableShippingMethodsAsync();
            return Json(shippingMethods);
        }

        // GET: storefrontapi/cart/paymentmethods
        [HttpGet("paymentmethods")]
        public async Task<ActionResult> GetCartAvailPaymentMethods()
        {
            EnsureCartExists();

            var cartBuilder = await LoadOrCreateCartAsync();

            var paymentMethods = await cartBuilder.GetAvailablePaymentMethodsAsync();
            return Json(paymentMethods);
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
                return Json(cartBuilder.Cart.Coupon);
            }
        }


        // DELETE: storefrontapi/cart/coupons
        [HttpDelete("coupons")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveCartCoupon()
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();
                await cartBuilder.RemoveCouponAsync();
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
        public async Task<ActionResult> CreateOrder([FromBody] orderModel.BankCardInfo bankCardInfo)
        {
            EnsureCartExists();

            //Need lock to prevent concurrent access to same cart
            using (await AsyncLock.GetLockByKey(WorkContext.CurrentCart.Value.GetCacheKey()).LockAsync())
            {
                var cartBuilder = await LoadOrCreateCartAsync();

                var order = await _orderApi.CreateOrderFromCartAsync(cartBuilder.Cart.Id);

                var taskList = new List<Task>
                {
                    //Raise domain event asynchronously
                    _publisher.Publish(new OrderPlacedEvent(WorkContext, order.ToCustomerOrder(WorkContext.AllCurrencies, WorkContext.CurrentLanguage), cartBuilder.Cart)),
                    //Remove the cart asynchronously
                    cartBuilder.RemoveCartAsync()
                };
                //Process order asynchronously
                var incomingPayment = order.InPayments != null ? order.InPayments.FirstOrDefault() : null;
                Task<orderModel.ProcessPaymentResult> processPaymentTask = null;
                if (incomingPayment != null)
                {
                    processPaymentTask = _orderApi.ProcessOrderPaymentsAsync(order.Id, incomingPayment.Id, bankCardInfo);
                    taskList.Add(processPaymentTask);
                }
                await Task.WhenAll(taskList.ToArray());

                return Json(new { order, orderProcessingResult = processPaymentTask != null ? await processPaymentTask : null, paymentMethod = incomingPayment != null ? incomingPayment.PaymentMethod : null });
            }
        }


        private void EnsureCartExists()
        {
            if (WorkContext.CurrentCart.Value == null)
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
