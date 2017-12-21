﻿using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CartModuleApi;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
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
using cartModel = VirtoCommerce.Storefront.AutoRestClients.CartModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public class CartBuilder : ICartBuilder
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICartModule _cartApi;
        private readonly ICatalogService _catalogService;
        private readonly IMemoryCache _memoryCache;
        private readonly IPromotionEvaluator _promotionEvaluator;
        private readonly ITaxEvaluator _taxEvaluator;
        private readonly ISubscriptionService _subscriptionService;

        public CartBuilder(IWorkContextAccessor workContextAccessor, ICartModule cartApi, ICatalogService catalogSearchService,
            IMemoryCache memoryCache, IPromotionEvaluator promotionEvaluator, ITaxEvaluator taxEvaluator, ISubscriptionService subscriptionService)
        {
            _cartApi = cartApi;
            _catalogService = catalogSearchService;
            _memoryCache = memoryCache;
            _workContextAccessor = workContextAccessor;
            _promotionEvaluator = promotionEvaluator;
            _taxEvaluator = taxEvaluator;
            _subscriptionService = subscriptionService;
        }

        #region ICartBuilder Members

        public virtual ShoppingCart Cart { get; private set; }

        public virtual Task TakeCartAsync(ShoppingCart cart)
        {
            Cart = cart;
            return Task.FromResult((object)null);
        }

        public void LoadOrCreateNewTransientCart(string cartName, Store store, User user, Language language, Currency currency)
        {
            Task.Factory.StartNew(() => LoadOrCreateNewTransientCartAsync(cartName, store, user, language, currency), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public virtual async Task LoadOrCreateNewTransientCartAsync(string cartName, Store store, User user, Language language, Currency currency)
        {
            var cacheKey = CacheKey.With(GetType(), store.Id, cartName, user.Id, currency.Code);
            var needReevaluate = false;
            Cart = await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
           {
               needReevaluate = true;

               var cartSearchCriteria = CreateCartSearchCriteria(cartName, store, user, language, currency);
               var cartSearchResult = await _cartApi.SearchAsync(cartSearchCriteria);

               var cartDto = cartSearchResult.Results.FirstOrDefault();
               var cart = cartDto?.ToShoppingCart(currency, language, user) ?? CreateCart(cartName, store, user, language, currency);

               //Load cart payment plan with have same id
               if (store.SubscriptionEnabled)
               {
                   var productsIds = cart.Items.Select(x => x.ProductId).Distinct().ToArray();
                   var products = await _catalogService.GetProductsAsync(productsIds, ItemResponseGroup.ItemWithPrices | ItemResponseGroup.ItemWithDiscounts | ItemResponseGroup.Inventory);
                   var paymentPlanIds = new[] { cart.Id }.Concat(cart.Items.Select(x => x.ProductId).Distinct()).ToArray();
                   var paymentPlans = await _subscriptionService.GetPaymentPlansByIdsAsync(paymentPlanIds);
                   cart.PaymentPlan = paymentPlans.FirstOrDefault(x => x.Id == cart.Id);
                   //Realize this code whith dictionary
                   foreach (var lineItem in cart.Items)
                   {
                       lineItem.Product = products.FirstOrDefault(x => x.Id == lineItem.ProductId);
                       lineItem.PaymentPlan = paymentPlans.FirstOrDefault(x => x.Id == lineItem.ProductId);
                   }
               }

               cart.Customer = user;

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
            var entities = cart.GetFlatObjectsListWithInterface<IEntity>();
            foreach (var entity in entities)
            {
                entity.Id = null;
            }

            foreach (var lineItem in cart.Items)
            {
                await AddLineItemAsync(lineItem);
            }
            Cart.Coupon = cart.Coupon;

            Cart.Shipments.Clear();
            Cart.Shipments = cart.Shipments;

            Cart.Payments.Clear();
            Cart.Payments = cart.Payments;
        }

        public virtual async Task RemoveCartAsync()
        {
            EnsureCartExists();
            //Evict cart from cache
            CartCacheRegion.ExpireCart(Cart);

            await _cartApi.DeleteCartsAsync(new List<string> { Cart.Id });
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
            var shippingRates = await _cartApi.GetAvailableShippingRatesAsync(Cart.Id);
            var retVal = shippingRates.Select(x => x.ToShippingMethod(Cart.Currency, workContext.AllCurrencies)).OrderBy(x => x.Priority).ToList();

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
            var payments = await _cartApi.GetAvailablePaymentMethodsAsync(Cart.Id);
            var retVal = payments.Select(x => x.ToPaymentMethod(Cart)).OrderBy(x => x.Priority).ToList();

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

            bool isReadOnlyLineItems = Cart.Items.Any(i => i.IsReadOnly);
            if (!isReadOnlyLineItems)
            {
                //Get product inventory to fill InStockQuantity parameter of LineItem
                //required for some promotions evaluation

                foreach (var lineItem in Cart.Items.ToList())
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
            //Evict cart from cache
            CartCacheRegion.ExpireCart(Cart);

            await EvaluatePromotionsAsync();
            await EvaluateTaxesAsync();

            var cart = Cart.ToShoppingCartDto();
            if (string.IsNullOrEmpty(cart.Id))
            {
                cart = await _cartApi.CreateAsync(cart);
            }
            else
            {
                await _cartApi.UpdateAsync(cart);
            }
            cart = await _cartApi.GetCartByIdAsync(cart.Id);
            Cart = cart.ToShoppingCart(Cart.Currency, Cart.Language, Cart.Customer);
        }

        #endregion      

        protected virtual cartModel.ShoppingCartSearchCriteria CreateCartSearchCriteria(string cartName, Store store, User user, Language language, Currency currency)
        {
            return new cartModel.ShoppingCartSearchCriteria
            {
                StoreId = store.Id,
                CustomerId = user.Id,
                Name = cartName,
                Currency = currency.Code,
            };
        }

        protected virtual ShoppingCart CreateCart(string cartName, Store store, User user, Language language, Currency currency)
        {
            var cart = new ShoppingCart(currency, language)
            {
                CustomerId = user.Id,
                Name = cartName,
                StoreId = store.Id,
                Language = language,
                IsAnonymous = !user.IsRegisteredUser,
                CustomerName = user.IsRegisteredUser ? user.UserName : StorefrontClaims.AnonymousUsername
            };

            return cart;
        }

        protected virtual async Task ValidateCartItemsAsync()
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

        protected virtual async Task ChangeItemQuantityAsync(LineItem lineItem, int quantity)
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
    }
}
