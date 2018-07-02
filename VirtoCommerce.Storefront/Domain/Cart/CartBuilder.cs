using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Storefront.Model.Subscriptions.Services;
using VirtoCommerce.Storefront.Model.Tax.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class CartBuilder : ICartBuilder
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalogService;
        private readonly IMemoryCache _memoryCache;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly ITaxEvaluator _taxEvaluator;
        private readonly ISubscriptionService _subscriptionService;

        public CartBuilder(IWorkContextAccessor workContextAccessor, ICartService cartService, ICatalogService catalogSearchService,
            IMemoryCache memoryCache, IPromotionEvaluator promotionEvaluator, ITaxEvaluator taxEvaluator, ISubscriptionService subscriptionService)
        {
            _cartService = cartService;
            _catalogService = catalogSearchService;
            _memoryCache = memoryCache;
            _workContextAccessor = workContextAccessor;
            _promotionEvaluator = promotionEvaluator;
            _taxEvaluator = taxEvaluator;
            _subscriptionService = subscriptionService;
        }

        #region ICartBuilder Members

        public virtual ShoppingCart Cart { get; private set; }

        public virtual async Task TakeCartAsync(ShoppingCart cart)
        {
            var store = _workContextAccessor.WorkContext.AllStores.FirstOrDefault(x => x.Id.EqualsInvariant(cart.StoreId));
            if (store == null)
            {
                throw new StorefrontException($"{ nameof(cart.StoreId) } not found");
            }
            //Load cart dependencies
            await PrepareCartAsync(cart, store);

            Cart = cart;
        }

        public void LoadOrCreateNewTransientCart(string cartName, Store store, User user, Language language, Currency currency, string type = null)
        {
            LoadOrCreateNewTransientCartAsync(cartName, store, user, language, currency).GetAwaiter().GetResult();
        }

        public virtual async Task LoadOrCreateNewTransientCartAsync(string cartName, Store store, User user, Language language, Currency currency, string type = null)
        {
            var cacheKey = CacheKey.With(GetType(), "LoadOrCreateNewTransientCart", store.Id, cartName, user.Id, currency.Code, type);
            var needReevaluate = false;
            Cart = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
            {
                needReevaluate = true;

                var cartSearchCriteria = CreateCartSearchCriteria(cartName, store, user, language, currency, type);
                var cartSearchResult = await _cartService.SearchCartsAsync(cartSearchCriteria);
                var cart = cartSearchResult.FirstOrDefault() ?? CreateCart(cartName, store, user, language, currency, type);

                //Load cart dependencies
                await PrepareCartAsync(cart, store);

                //Add expiration token for concrete cart instance
                cacheEntry.AddExpirationToken(CartCacheRegion.CreateChangeToken(cart));

                return cart;
            });

            if (needReevaluate)
            {
                await EvaluatePromotionsAsync();
                await EvaluateTaxesAsync();
            }
        }

        public virtual Task UpdateCartComment(string comment)
        {
            EnsureCartExists();

            Cart.Comment = comment;

            return Task.CompletedTask;
        }

        public virtual async Task AddItemAsync(Product product, int quantity)
        {
            EnsureCartExists();

            var isProductAvailable = new ProductIsAvailableSpecification(product).IsSatisfiedBy(quantity);
            if (isProductAvailable)
            {
                var lineItem = product.ToLineItem(Cart.Language, quantity);
                lineItem.Product = product;
                await AddLineItemAsync(lineItem);
            }
        }

        public virtual async Task ChangeItemQuantityAsync(string id, int quantity)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == id);
            if (lineItem != null)
            {
                await ChangeItemQuantityAsync(lineItem, quantity);
            }
        }

        public virtual async Task ChangeItemQuantityAsync(int lineItemIndex, int quantity)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.ElementAt(lineItemIndex);
            if (lineItem != null)
            {
                await ChangeItemQuantityAsync(lineItem, quantity);
            }
        }

        public virtual async Task ChangeItemsQuantitiesAsync(int[] quantities)
        {
            EnsureCartExists();

            for (var i = 0; i < quantities.Length; i++)
            {
                var lineItem = Cart.Items.ElementAt(i);
                if (lineItem != null && quantities[i] > 0)
                {
                    await ChangeItemQuantityAsync(lineItem, quantities[i]);
                }
            }
        }

        public virtual Task RemoveItemAsync(string id)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(x => x.Id == id);
            if (lineItem != null)
            {
                Cart.Items.Remove(lineItem);
            }

            return Task.FromResult((object)null);
        }

        public virtual Task AddCouponAsync(string couponCode)
        {
            EnsureCartExists();
            Cart.Coupon = new Coupon { Code = couponCode };
            return Task.FromResult((object)null);
        }

        public virtual Task RemoveCouponAsync()
        {
            EnsureCartExists();
            Cart.Coupon = null;
            return Task.FromResult((object)null);
        }

        public virtual Task ClearAsync()
        {
            EnsureCartExists();
            Cart.Items.Clear();
            return Task.FromResult((object)null);
        }

        public virtual async Task AddOrUpdateShipmentAsync(Shipment shipment)
        {
            EnsureCartExists();

            await RemoveExistingShipmentAsync(shipment);

            shipment.Currency = Cart.Currency;
            if (shipment.DeliveryAddress != null)
            {
                //Reset address key because it can equal a customer address from profile and if not do that it may cause
                //address primary key duplication error for multiple carts with the same address 
                shipment.DeliveryAddress.Key = null;
            }
            Cart.Shipments.Add(shipment);


            if (!string.IsNullOrEmpty(shipment.ShipmentMethodCode))
            {
                var availableShippingMethods = await GetAvailableShippingMethodsAsync();
                var shippingMethod = availableShippingMethods.FirstOrDefault(sm => shipment.ShipmentMethodCode.EqualsInvariant(sm.ShipmentMethodCode) && shipment.ShipmentMethodOption.EqualsInvariant(sm.OptionName));
                if (shippingMethod == null)
                {
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "Unknown shipment method: {0} with option: {1}", shipment.ShipmentMethodCode, shipment.ShipmentMethodOption));
                }
                shipment.Price = shippingMethod.Price;
                shipment.DiscountAmount = shippingMethod.DiscountAmount;
                shipment.TaxType = shippingMethod.TaxType;
            }
        }

        public virtual Task RemoveShipmentAsync(string shipmentId)
        {
            EnsureCartExists();

            var shipment = Cart.Shipments.FirstOrDefault(s => s.Id == shipmentId);
            if (shipment != null)
            {
                Cart.Shipments.Remove(shipment);
            }

            return Task.FromResult((object)null);
        }

        public virtual async Task AddOrUpdatePaymentAsync(Payment payment)
        {
            EnsureCartExists();

            await RemoveExistingPaymentAsync(payment);
            if (payment.BillingAddress != null)
            {
                //Reset address key because it can equal a customer address from profile and if not do that it may cause
                //address primary key duplication error for multiple carts with the same address 
                payment.BillingAddress.Key = null;
            }
            Cart.Payments.Add(payment);

            if (!string.IsNullOrEmpty(payment.PaymentGatewayCode))
            {
                var availablePaymentMethods = await GetAvailablePaymentMethodsAsync();
                var paymentMethod = availablePaymentMethods.FirstOrDefault(pm => string.Equals(pm.Code, payment.PaymentGatewayCode, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethod == null)
                {
                    throw new Exception("Unknown payment method " + payment.PaymentGatewayCode);
                }
            }
        }

        public virtual async Task MergeWithCartAsync(ShoppingCart cart)
        {
            EnsureCartExists();

            //Reset primary keys for all aggregated entities before merge
            //To prevent insertions same Ids for target cart
            //exclude user because it might be the current one
            var entities = cart.GetFlatObjectsListWithInterface<IEntity>();
            foreach (var entity in entities.Where(x => !(x is User)).ToList())
            {
                entity.Id = null;
            }

            foreach (var lineItem in cart.Items)
            {
                await AddLineItemAsync(lineItem);
            }

            if (cart.Coupon != null)
            {
                Cart.Coupon = cart.Coupon;
            }

            foreach (var shipment in cart.Shipments)
            {
                await AddOrUpdateShipmentAsync(shipment);
            }

            foreach (var payment in cart.Payments)
            {
                await AddOrUpdatePaymentAsync(payment);
            }
        }

        public virtual async Task RemoveCartAsync()
        {
            EnsureCartExists();
            //Evict cart from cache
            CartCacheRegion.ExpireCart(Cart);
            await _cartService.DeleteCartByIdAsync(Cart.Id);
        }

        public virtual async Task FillFromQuoteRequestAsync(QuoteRequest quoteRequest)
        {
            EnsureCartExists();

            var productIds = quoteRequest.Items.Select(i => i.ProductId);
            var products = await _catalogService.GetProductsAsync(productIds.ToArray(), ItemResponseGroup.ItemLarge);

            Cart.Items.Clear();
            foreach (var product in products)
            {
                var quoteItem = quoteRequest.Items.FirstOrDefault(i => i.ProductId == product.Id);
                if (quoteItem != null)
                {
                    var lineItem = product.ToLineItem(Cart.Language, (int)quoteItem.SelectedTierPrice.Quantity);
                    lineItem.Product = product;
                    lineItem.ListPrice = quoteItem.ListPrice;
                    lineItem.SalePrice = quoteItem.SelectedTierPrice.Price;
                    if (lineItem.ListPrice < lineItem.SalePrice)
                    {
                        lineItem.ListPrice = lineItem.SalePrice;
                    }
                    lineItem.DiscountAmount = lineItem.ListPrice - lineItem.SalePrice;
                    lineItem.IsReadOnly = true;
                    lineItem.Id = null;
                    Cart.Items.Add(lineItem);
                }
            }

            if (quoteRequest.RequestShippingQuote)
            {
                Cart.Shipments.Clear();
                var shipment = new Shipment(Cart.Currency);

                if (quoteRequest.ShippingAddress != null)
                {
                    shipment.DeliveryAddress = quoteRequest.ShippingAddress;
                }

                if (quoteRequest.ShipmentMethod != null)
                {
                    var availableShippingMethods = await GetAvailableShippingMethodsAsync();
                    var availableShippingMethod = availableShippingMethods?.FirstOrDefault(sm => sm.ShipmentMethodCode == quoteRequest.ShipmentMethod.ShipmentMethodCode);

                    if (availableShippingMethod != null)
                    {
                        shipment = quoteRequest.ShipmentMethod.ToCartShipment(Cart.Currency);
                    }
                }
                Cart.Shipments.Add(shipment);
            }

            var payment = new Payment(Cart.Currency)
            {
                Amount = quoteRequest.Totals.GrandTotalInclTax
            };

            if (quoteRequest.BillingAddress != null)
            {
                payment.BillingAddress = quoteRequest.BillingAddress;
            }

            Cart.Payments.Clear();
            Cart.Payments.Add(payment);
        }

        public virtual async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync()
        {
            var workContext = _workContextAccessor.WorkContext;

            //Request available shipping rates 
            var retVal = await _cartService.GetAvailableShippingMethodsAsync(Cart);

            //Evaluate promotions cart and apply rewards for available shipping methods
            var promoEvalContext = Cart.ToPromotionEvaluationContext();
            await _promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, retVal);

            //Evaluate taxes for available shipping rates
            var taxEvalContext = Cart.ToTaxEvalContext(workContext.CurrentStore);
            taxEvalContext.Lines.Clear();
            taxEvalContext.Lines.AddRange(retVal.SelectMany(x => x.ToTaxLines()));
            await _taxEvaluator.EvaluateTaxesAsync(taxEvalContext, retVal);

            return retVal;
        }

        public virtual async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync()
        {
            EnsureCartExists();
            var retVal = await _cartService.GetAvailablePaymentMethodsAsync(Cart);

            //Evaluate promotions cart and apply rewards for available shipping methods
            var promoEvalContext = Cart.ToPromotionEvaluationContext();
            await _promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, retVal);

            //Evaluate taxes for available payments 
            var workContext = _workContextAccessor.WorkContext;
            var taxEvalContext = Cart.ToTaxEvalContext(workContext.CurrentStore);
            taxEvalContext.Lines.Clear();
            taxEvalContext.Lines.AddRange(retVal.SelectMany(x => x.ToTaxLines()));
            await _taxEvaluator.EvaluateTaxesAsync(taxEvalContext, retVal);

            return retVal;
        }

        public async Task ValidateAsync()
        {
            EnsureCartExists();
            await Task.WhenAll(ValidateCartItemsAsync(), ValidateCartShipmentsAsync());
            Cart.IsValid = Cart.Items.All(x => x.IsValid) && Cart.Shipments.All(x => x.IsValid);
        }

        public virtual async Task EvaluatePromotionsAsync()
        {
            EnsureCartExists();

            var isReadOnlyLineItems = Cart.Items.Any(i => i.IsReadOnly);
            if (!isReadOnlyLineItems)
            {
                //Get product inventory to fill InStockQuantity parameter of LineItem
                //required for some promotions evaluation

                foreach (var lineItem in Cart.Items.Where(x => x.Product != null).ToList())
                {
                    lineItem.InStockQuantity = (int)lineItem.Product.AvailableQuantity;
                }

                var evalContext = Cart.ToPromotionEvaluationContext();
                await _promotionEvaluator.EvaluateDiscountsAsync(evalContext, new IDiscountable[] { Cart });
            }
        }

        public async Task EvaluateTaxesAsync()
        {
            var workContext = _workContextAccessor.WorkContext;
            await _taxEvaluator.EvaluateTaxesAsync(Cart.ToTaxEvalContext(workContext.CurrentStore), new[] { Cart });
        }

        public virtual async Task SaveAsync()
        {
            EnsureCartExists();

            await EvaluatePromotionsAsync();
            await EvaluateTaxesAsync();

            var cart = await _cartService.SaveChanges(Cart);
            //Evict cart from cache
            CartCacheRegion.ExpireCart(Cart);

            await TakeCartAsync(cart);
        }

        #endregion

        protected virtual CartSearchCriteria CreateCartSearchCriteria(string cartName, Store store, User user, Language language, Currency currency, string type)
        {
            return new CartSearchCriteria
            {
                StoreId = store.Id,
                Customer = user,
                Name = cartName,
                Currency = currency,
                Type = type
            };
        }

        protected virtual ShoppingCart CreateCart(string cartName, Store store, User user, Language language, Currency currency, string type)
        {
            var cart = new ShoppingCart(currency, language)
            {
                CustomerId = user.Id,
                Name = cartName,
                StoreId = store.Id,
                Language = language,
                Customer = user,
                Type = type,
                IsAnonymous = !user.IsRegisteredUser,
                CustomerName = user.IsRegisteredUser ? user.UserName : SecurityConstants.AnonymousUsername
            };

            return cart;
        }

        protected virtual Task ValidateCartItemsAsync()
        {
            foreach (var lineItem in Cart.Items.ToList())
            {
                lineItem.ValidationErrors.Clear();

                if (lineItem.Product == null || !lineItem.Product.IsActive || !lineItem.Product.IsBuyable)
                {
                    lineItem.ValidationErrors.Add(new UnavailableError());
                    lineItem.IsValid = false;
                }
                else
                {
                    var isProductAvailable = new ProductIsAvailableSpecification(lineItem.Product).IsSatisfiedBy(lineItem.Quantity);
                    if (!isProductAvailable)
                    {
                        lineItem.IsValid = false;

                        var availableQuantity = lineItem.Product.AvailableQuantity;
                        lineItem.ValidationErrors.Add(new QuantityError(availableQuantity));
                    }

                    var tierPrice = lineItem.Product.Price.GetTierPrice(lineItem.Quantity);
                    if (tierPrice.Price > lineItem.SalePrice)
                    {
                        lineItem.ValidationErrors.Add(new PriceError(lineItem.SalePrice, lineItem.SalePriceWithTax, tierPrice.Price, tierPrice.PriceWithTax));
                    }
                }
            }
            return Task.CompletedTask;
        }

        protected virtual async Task ValidateCartShipmentsAsync()
        {
            foreach (var shipment in Cart.Shipments.ToArray())
            {
                shipment.ValidationErrors.Clear();

                var availShippingmethods = await GetAvailableShippingMethodsAsync();
                var shipmentShippingMethod = availShippingmethods.FirstOrDefault(sm => shipment.HasSameMethod(sm));
                if (shipmentShippingMethod == null)
                {
                    shipment.ValidationErrors.Add(new UnavailableError());
                }
                else if (shipmentShippingMethod.Price != shipment.Price)
                {
                    shipment.ValidationErrors.Add(new PriceError(shipment.Price, shipment.PriceWithTax, shipmentShippingMethod.Price, shipmentShippingMethod.PriceWithTax));
                }
            }
        }

        protected virtual Task RemoveExistingPaymentAsync(Payment payment)
        {
            if (payment != null)
            {
                var existingPayment = !payment.IsTransient() ? Cart.Payments.FirstOrDefault(s => s.Id == payment.Id) : null;
                if (existingPayment != null)
                {
                    Cart.Payments.Remove(existingPayment);
                }
            }

            return Task.FromResult((object)null);
        }

        protected virtual Task RemoveExistingShipmentAsync(Shipment shipment)
        {
            if (shipment != null)
            {
                var existShipment = !shipment.IsTransient() ? Cart.Shipments.FirstOrDefault(s => s.Id == shipment.Id) : null;
                if (existShipment != null)
                {
                    Cart.Shipments.Remove(existShipment);
                }
            }

            return Task.FromResult((object)null);
        }

        protected virtual Task ChangeItemQuantityAsync(LineItem lineItem, int quantity)
        {
            if (lineItem != null && !lineItem.IsReadOnly)
            {
                if (lineItem.Product != null)
                {
                    lineItem.SalePrice = lineItem.Product.Price.GetTierPrice(quantity).Price;
                    //List price should be always greater ot equals sale price because it may cause incorrect totals calculation
                    if (lineItem.ListPrice < lineItem.SalePrice)
                    {
                        lineItem.ListPrice = lineItem.SalePrice;
                    }
                }
                if (quantity > 0)
                {
                    lineItem.Quantity = quantity;
                }
                else
                {
                    Cart.Items.Remove(lineItem);
                }
            }
            return Task.CompletedTask;
        }

        protected virtual async Task AddLineItemAsync(LineItem lineItem)
        {
            var existingLineItem = Cart.Items.FirstOrDefault(li => li.ProductId == lineItem.ProductId);
            if (existingLineItem != null)
            {
                await ChangeItemQuantityAsync(existingLineItem, existingLineItem.Quantity + Math.Max(1, lineItem.Quantity));
            }
            else
            {
                lineItem.Id = null;
                Cart.Items.Add(lineItem);
            }
        }

        protected virtual void EnsureCartExists()
        {
            if (Cart == null)
            {
                throw new StorefrontException("Cart not loaded.");
            }
        }

        protected virtual async Task PrepareCartAsync(ShoppingCart cart, Store store)
        {
            if (cart == null)
            {
                throw new ArgumentNullException(nameof(cart));
            }

            //Load products for cart line items
            if (cart.Items.Any())
            {
                var productIds = cart.Items.Select(i => i.ProductId).ToArray();
                var products = await _catalogService.GetProductsAsync(productIds, ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.Inventory);
                foreach (var item in cart.Items)
                {
                    item.Product = products.FirstOrDefault(x => x.Id.EqualsInvariant(item.ProductId));
                }
            }

            //Load cart payment plan with have same id
            if (store.SubscriptionEnabled)
            {
                var paymentPlanIds = new[] { cart.Id }.Concat(cart.Items.Select(x => x.ProductId).Distinct()).ToArray();
                var paymentPlans = await _subscriptionService.GetPaymentPlansByIdsAsync(paymentPlanIds);
                cart.PaymentPlan = paymentPlans.FirstOrDefault(x => x.Id == cart.Id);
                //Realize this code whith dictionary
                foreach (var lineItem in cart.Items)
                {
                    lineItem.PaymentPlan = paymentPlans.FirstOrDefault(x => x.Id == lineItem.ProductId);
                }
            }
        }
    }
}
