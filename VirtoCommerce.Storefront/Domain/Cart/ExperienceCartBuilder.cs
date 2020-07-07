using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using GraphQL;
using GraphQL.Client.Abstractions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Cart.Services;
using VirtoCommerce.Storefront.Model.Cart.Validators;
using VirtoCommerce.Storefront.Model.Commands;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.Contracts;
using VirtoCommerce.Storefront.Model.Quote;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain.Cart
{
    public class ExperienceCartBuilder : ICartBuilder
    {
        private readonly IGraphQLClient _client;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ExperienceCartBuilder(IWorkContextAccessor workContextAccessor, IGraphQLClient client)
        {
            _client = client;
            _workContextAccessor = workContextAccessor;
        }

        public virtual ShoppingCart Cart { get; protected set; }

        public virtual async Task AddCouponAsync(string couponCode)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.AddCoupon(),
                Variables = new
                {
                    Command = new AddCouponCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        CouponCode = couponCode
                    }
                }
            };

            var response = await _client.SendMutationAsync<AddCouponResponseDto>(request);

            Cart = response.Data.AddCoupon.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public virtual async Task<bool> AddItemAsync(AddCartItem addCartItem)
        {
            var result = await new AddCartItemValidator(Cart).ValidateAsync(addCartItem, ruleSet: Cart.ValidationRuleSet);
            if (result.IsValid)
            {
                var request = new GraphQLRequest
                {
                    Query = QueryHelper.AddItemToCart(),
                    Variables = new
                    {
                        Command = new AddCartItemCommand
                        {
                            StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                            CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                            UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                            Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                            Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                            CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                            ProductId = addCartItem.ProductId,
                            Quantity = addCartItem.Quantity
                        }
                    }
                };

                var response = await _client.SendMutationAsync<AddItemResponseDto>(request);

                Cart = response.Data.AddItem.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
            }
            return result.IsValid;
        }

        public async Task AddOrUpdatePaymentAsync(Payment payment)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.AddOrUpdatePayment(),
                Variables = new
                {
                    Command = new AddOrUpdateCartPaymentCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        Payment = payment.ToDto()
                    }
                }
            };

            var response = await _client.SendMutationAsync<AddOrUpdateCartPaymentResponseDto>(request);

            Cart = response.Data.AddOrUpdateCartPayment.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task AddOrUpdateShipmentAsync(Shipment shipment)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.AddOrUpdateShippment(),
                Variables = new
                {
                    Command = new AddOrUpdateShipmentCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        Shipment = shipment.ToDto()
                    }
                }
            };

            var response = await _client.SendMutationAsync<AddOrUpdateCartShipmentResponseDto>(request);

            Cart = response.Data.AddOrUpdateCartShipment.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task ChangeItemCommentAsync(ChangeCartItemComment newItemComment)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == newItemComment.LineItemId);
            if (lineItem == null)
            {
                return;
            }

            var request = new GraphQLRequest
            {
                Query = QueryHelper.ChangeCartItemComment(),
                Variables = new
                {
                    Command = new ChangeCartItemCommentCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        LineItemId = newItemComment.LineItemId,
                        Comment = newItemComment.Comment,
                    }
                }
            };

            var response = await _client.SendMutationAsync<ShoppingCartDtoContainer>(request);

            Cart = response.Data.ShoppingCartDto.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public Task ChangeItemDynamicPropertiesAsync(ChangeCartItemDynamicProperties newItemDynamicProperties)
        {
            throw new NotImplementedException();
        }

        public async Task ChangeItemPriceAsync(ChangeCartItemPrice newPrice)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.ChangeCartItemPrice(),
                Variables = new
                {
                    Command = new ChangeCartItemPriceCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        ProductId = newPrice.LineItemId,
                        Price = newPrice.NewPrice
                    }
                }
            };

            var response = await _client.SendMutationAsync<ChangeCartItemPriceResponseDto>(request);

            Cart = response.Data.ChangeCartItemPrice.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task ChangeItemQuantityAsync(ChangeCartItemQty changeItemQty)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.FirstOrDefault(i => i.Id == changeItemQty.LineItemId);
            if (lineItem == null)
            {
                return;
            }

            await ChangeLineItemQuantity(changeItemQty.LineItemId, changeItemQty.Quantity);
        }

        public async Task ChangeItemQuantityAsync(int lineItemIndex, int quantity)
        {
            EnsureCartExists();

            var lineItem = Cart.Items.ElementAt(lineItemIndex);
            if (lineItem == null)
            {
                return;
            }

            await ChangeLineItemQuantity(lineItem.Id, quantity);
        }

        public async Task ChangeItemsQuantitiesAsync(int[] quantities)
        {
            EnsureCartExists();

            for (var i = 0; i < quantities.Length; i++)
            {
                var lineItem = Cart.Items.ElementAt(i);
                if (lineItem != null && quantities[i] > 0)
                {
                    await ChangeItemQuantityAsync(new ChangeCartItemQty
                    {
                        LineItemId = lineItem.Id,
                        Quantity = quantities[i],
                    });
                }
            }
        }

        public virtual async Task ClearAsync()
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.ClearCart(),
                Variables = new
                {
                    Command = new ClearCartCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                    }
                }
            };

            var response = await _client.SendMutationAsync<ClearCartResponseDto>(request);

            Cart = response.Data.ClearCart.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public Task EvaluatePromotionsAsync()
        {
            return Task.CompletedTask;
        }

        public Task EvaluateTaxesAsync()
        {
            return Task.CompletedTask;
        }

        public Task FillFromQuoteRequestAsync(QuoteRequest quoteRequest)
        {
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<PaymentMethod>> GetAvailablePaymentMethodsAsync()
        {
            var query = QueryHelper.GetCart(
                storeId: _workContextAccessor.WorkContext.CurrentStore.Id,
                cartName: _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                userId: _workContextAccessor.WorkContext.CurrentUser.Id,
                cultureName: _workContextAccessor.WorkContext.CurrentLanguage?.CultureName ?? "en-US",
                currencyCode: _workContextAccessor.WorkContext.CurrentCurrency.Code,
                type: _workContextAccessor.WorkContext.CurrentCart.Value.Type ?? string.Empty,
                selectedFields: QueryHelper.AvailablePaymentMethods());

            var response = await _client.SendQueryAsync<GetCartResponseDto>(new GraphQLRequest { Query = query });

            return response.Data.Cart.AvailablePaymentMethods;
        }

        public async Task<IEnumerable<ShippingMethod>> GetAvailableShippingMethodsAsync()
        {
            var query = QueryHelper.GetCart(
                storeId: _workContextAccessor.WorkContext.CurrentStore.Id,
                cartName: _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                userId: _workContextAccessor.WorkContext.CurrentUser.Id,
                cultureName: _workContextAccessor.WorkContext.CurrentLanguage?.CultureName ?? "en-US",
                currencyCode: _workContextAccessor.WorkContext.CurrentCurrency.Code,
                type: _workContextAccessor.WorkContext.CurrentCart.Value.Type ?? string.Empty,
                selectedFields: QueryHelper.AvailableShippingMethods());

            var response = await _client.SendQueryAsync<GetCartResponseDto>(new GraphQLRequest { Query = query });

            return response.Data.Cart.AvailableShippingMethods;
        }

        public void LoadOrCreateNewTransientCart(string cartName, Store store, User user, Language language, Currency currency, string type = null)
        {
            LoadOrCreateNewTransientCartAsync(cartName, store, user, language, currency).GetAwaiter().GetResult();
        }

        public virtual async Task LoadOrCreateNewTransientCartAsync(string cartName, Store store, User user, Language language, Currency currency, string type = null)
        {
            var query = QueryHelper.GetCart(
                storeId: store.Id,
                cartName: cartName,
                userId: user?.Id,
                cultureName: language?.CultureName ?? "en-US",
                currencyCode: currency.Code,
                type: type ?? string.Empty);

            var response = await _client.SendQueryAsync<GetCartResponseDto>(new GraphQLRequest { Query = query });

            Cart = response.Data.Cart.ToShoppingCart(currency, language, user);
        }

        public async Task MergeWithCartAsync(ShoppingCart cart)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.MergeCartType(),
                Variables = new
                {
                    Command = new MergeCartCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = Cart.CustomerId.ToString(),
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        SecondCartId = cart.Id
                    }
                }
            };

            var response = await _client.SendMutationAsync<MergeCartResponseDto>(request);

            Cart = response.Data.MergeCart.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task RemoveCartAsync(string cartId = null)
        {
            // Guardian, nullable argument added for backward compatibility
            if (cartId == null)
            {
                return;
            }

            var request = new GraphQLRequest
            {
                Query = QueryHelper.RemoveCartType(),
                Variables = new
                {
                    Command = new RemoveCartCommand
                    {
                        CartId = cartId
                    }
                }
            };

            await _client.SendMutationAsync<bool>(request);
        }

        public async Task RemoveCouponAsync(string couponCode = null)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.RemoveCouponMutation(),
                Variables = new
                {
                    Command = new RemoveCouponCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        CouponCode = couponCode
                    }
                }
            };

            var response = await _client.SendMutationAsync<ShoppingCartDtoContainer>(request);

            Cart = response.Data.ShoppingCartDto.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task RemoveItemAsync(string lineItemId)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.RemoveCartItem(),
                Variables = new
                {
                    Command = new RemoveCartItemCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        LineItemId = lineItemId
                    }
                }
            };

            var response = await _client.SendMutationAsync<RemoveCartItemResponseDto>(request);

            Cart = response.Data.RemoveCartItem.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task RemoveShipmentAsync(string shipmentId)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.RemoveShipmentMutation(),
                Variables = new
                {
                    Command = new RemoveShipmentCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        ShipmentId = shipmentId
                    }
                }
            };

            var response = await _client.SendMutationAsync<ShoppingCartDtoContainer>(request);

            Cart = response.Data.ShoppingCartDto.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        /// <summary>
        /// Backward compatibility
        /// </summary>
        [Obsolete("Do not use this method")]
        public Task SaveAsync() => Task.CompletedTask;

        /// <summary>
        /// Backward compatibility
        /// </summary>
        [Obsolete("Do not use this method")]
        public Task TakeCartAsync(ShoppingCart cart) => Task.CompletedTask;

        public async Task UpdateCartComment(string comment)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.ChangeCartComment(),
                Variables = new
                {
                    Command = new ChangeCartCommentCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        Comment = comment
                    }
                }
            };

            var response = await _client.SendMutationAsync<ChangeCartCommentResponseDto>(request);

            Cart = response.Data.ChangeComment.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }

        public async Task ValidateAsync()
        {
            EnsureCartExists();
            //TODO: implement cart validator
            //var result = await new CartValidator(_cartService).ValidateAsync(Cart, ruleSet: Cart.ValidationRuleSet);
            //Cart.IsValid = result.IsValid;
            Cart.IsValid = await Task.FromResult(true);
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

        private async Task ChangeLineItemQuantity(string lineItemId, int quantity)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.ChangeCartItemQuantity(),
                Variables = new
                {
                    Command = new ChangeCartItemQuantityCommand
                    {
                        StoreId = _workContextAccessor.WorkContext.CurrentStore.Id,
                        CartName = _workContextAccessor.WorkContext.CurrentCart.Value.Name,
                        UserId = _workContextAccessor.WorkContext.CurrentUser.Id,
                        Language = _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                        Currency = _workContextAccessor.WorkContext.CurrentCurrency.Code,
                        CartType = _workContextAccessor.WorkContext.CurrentCart.Value.Type,
                        LineItemId = lineItemId,
                        Quantity = quantity
                    }
                }
            };

            var response = await _client.SendMutationAsync<ChangeCartItemQuantityResponseDto>(request);

            Cart = response.Data.ChangeCartItemQuantity.ToShoppingCart(_workContextAccessor.WorkContext.CurrentCurrency, _workContextAccessor.WorkContext.CurrentLanguage, _workContextAccessor.WorkContext.CurrentUser);
        }
    }
}
