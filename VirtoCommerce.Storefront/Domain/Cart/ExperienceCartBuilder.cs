using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Commands;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain.Cart
{
    public class ExperienceCartBuilder : ICartBuilder
    {
        private readonly IGraphQlService _graphQlService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ExperienceCartBuilder(IWorkContextAccessor workContextAccessor, IGraphQlService graphQlService)
        {
            _graphQlService = graphQlService;
            _workContextAccessor = workContextAccessor;
        }

        public virtual ShoppingCart Cart { get; protected set; }

        public Task AddCouponAsync(string couponCode)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> AddItemAsync(AddCartItem addCartItem)
        {
            var result = await new AddCartItemValidator(Cart).ValidateAsync(addCartItem, ruleSet: Cart.ValidationRuleSet);
            if (result.IsValid)
            {
                var command = new AddCartItemCommand
                {
                    StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                    CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                    UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                    Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                    Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                    CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                    ProductId = addCartItem.ProductId,
                    Quantity = addCartItem.Quantity
                };

                var modifiedCart = await _graphQlService.AddItemToCartAsync(command);

                Cart = modifiedCart.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
            }
            return result.IsValid;
        }

        public Task AddOrUpdatePaymentAsync(Payment payment)
        {
            throw new NotImplementedException();
        }

        public Task AddOrUpdateShipmentAsync(Shipment shipment)
        {
            throw new NotImplementedException();
        }

        public Task ChangeItemCommentAsync(ChangeCartItemComment newItemComment)
        {
            throw new NotImplementedException();
        }

        public Task ChangeItemDynamicPropertiesAsync(ChangeCartItemDynamicProperties newItemDynamicProperties)
        {
            throw new NotImplementedException();
        }

        public Task ChangeItemPriceAsync(ChangeCartItemPrice newPrice)
        {
            throw new NotImplementedException();
        }

        public Task ChangeItemQuantityAsync(ChangeCartItemQty changeItemQty)
        {
            throw new NotImplementedException();
        }

        public Task ChangeItemQuantityAsync(int lineItemIndex, int quantity)
        {
            throw new NotImplementedException();
        }

        public Task ChangeItemsQuantitiesAsync(int[] quantities)
        {
            throw new NotImplementedException();
        }

        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task EvaluatePromotionsAsync()
        {
            throw new NotImplementedException();
        }

        public Task EvaluateTaxesAsync()
        {
            throw new NotImplementedException();
        }

        public Task FillFromQuoteRequestAsync(QuoteRequest quoteRequest)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync()
        {
            throw new NotImplementedException();
        }

        public void LoadOrCreateNewTransientCart(string cartName, Store store, User user, Language language, Currency currency, string type = null)
        {
            LoadOrCreateNewTransientCartAsync(cartName, store, user, language, currency).GetAwaiter().GetResult();
        }

        public async Task LoadOrCreateNewTransientCartAsync(string cartName, Store store, User user, Language language, Currency currency, string type = null)
        {
            var cartSearchCriteria = CreateCartSearchCriteria(cartName, store, user, language, currency, type);
            var cartDto = await _graphQlService.SearchShoppingCartAsync(cartSearchCriteria);

            Cart = cartDto.ToShoppingCart(currency, language, user);
        }

        public Task MergeWithCartAsync(ShoppingCart cart)
        {
            throw new NotImplementedException();
        }

        public Task RemoveCartAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveCouponAsync(string couponCode = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveItemAsync(string lineItemId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveShipmentAsync(string shipmentId)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task TakeCartAsync(ShoppingCart cart)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCartComment(string comment)
        {
            throw new NotImplementedException();
        }

        public Task ValidateAsync()
        {
            throw new NotImplementedException();
        }

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

        protected virtual void EnsureCartExists()
        {
            if (Cart == null)
            {
                throw new StorefrontException("Cart not loaded.");
            }
        }
    }
}
