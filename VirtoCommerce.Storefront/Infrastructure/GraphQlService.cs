using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Query.Builder;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Cart;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class GraphQlService : IGraphQlService
    {
        private readonly IGraphQLClient _client;

        public GraphQlService(IGraphQLClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ShoppingCartDto>> SearchShoppingCartAsync(Model.Cart.CartSearchCriteria criteria)
        {
            var builder = GetCartQuery(criteria);

            var query = "{" + builder.Build() + "}";

            var result = await _client.SendQueryAsync(
                new GraphQLRequest { Query = query },
                () => new
                {
                    cart = new ShoppingCartDto()
                });

            return new[] { result.Data.cart };
        }

        private static IQuery<ShoppingCartDto> GetCartQuery(Model.Cart.CartSearchCriteria criteria)
        {
            var builder = new Query<ShoppingCartDto>("cart", new QueryOptions { Formatter = QueryFormatters.CamelCaseFormatter })
                .AddArguments(new
                {
                    storeId = criteria.StoreId,
                    cartName = criteria.Name,
                    userId = criteria.Customer?.Id,
                    cultureName = criteria.Language?.CultureName ?? "en-US",
                    currencyCode = criteria.Currency.Code,
                    type = criteria.Type ?? string.Empty
                })
                .AddField(x => x.Id)
                .AddField(x => x.Name)
                .AddField(x => x.Status)
                .AddField(x => x.StoreId)
                .AddField(x => x.ChannelId)
                .AddField(x => x.HasPhysicalProducts)
                .AddField(x => x.IsAnonymous)
                //.AddField(x => x.Customer, nullable: true).Description("Shopping cart user"); //todo: add resolver
                .AddField(x => x.CustomerId)
                .AddField(x => x.CustomerName)
                .AddField(x => x.OrganizationId)
                .AddField(x => x.IsRecuring)
                .AddField(x => x.Comment)

                // Characteristics
                .AddField(x => x.VolumetricWeight)
                .AddField(x => x.WeightUnit)
                .AddField(x => x.Weight)
                .AddField(x => x.MeasureUnit)
                .AddField(x => x.Height)
                .AddField(x => x.Length)
                .AddField(x => x.Width)

                // Money
                .AddMoneyField(x => x.Total)
                .AddMoneyField(x => x.SubTotal)
                .AddMoneyField(x => x.SubTotalWithTax)
                .AddCurrencyField(x => x.Currency)
                .AddMoneyField(x => x.TaxTotal)
                .AddField(x => x.TaxPercentRate)
                .AddField(x => x.TaxType)

                //TODO: fix 
                .AddField(
                    x => x.TaxDetails,
                    t => t
                        .AddMoneyField(v => v.Rate)
                        .AddMoneyField(v => v.Amount)
                        //.AddField(v => v.Name) //TODO: fix "GraphQL.Validation.ValidationError: Field name of type MoneyType must have a sub selection"
                        //.AddField(v => v.Price) //TODO: fix "GraphQL.Validation.ValidationError: Field price of type MoneyType must have a sub selection"
                        )

                // Shipping
                .AddMoneyField(x => x.ShippingPrice)
                .AddMoneyField(x => x.ShippingPriceWithTax)
                .AddMoneyField(x => x.ShippingTotal)
                .AddMoneyField(x => x.ShippingTotalWithTax)
                .AddField(
                    x => x.Shipments,
                    t => t
                       .AddField(v => v.ShipmentMethodCode)
                       .AddField(v => v.ShipmentMethodOption)
                       .AddField(v => v.FulfillmentCenterId)
                       .AddAddressField(v => v.DeliveryAddress)
                       .AddField(x => x.VolumetricWeight)
                       .AddField(x => x.WeightUnit)
                       .AddField(x => x.Weight)
                       .AddField(x => x.MeasureUnit)
                       .AddField(x => x.Height)
                       .AddField(x => x.Length)
                       .AddField(x => x.Width)
                       .AddMoneyField(x => x.Price)
                       .AddMoneyField(x => x.PriceWithTax)
                       .AddMoneyField(x => x.Total)
                       .AddMoneyField(x => x.TotalWithTax)
                       .AddMoneyField(x => x.DiscountAmount)
                       .AddMoneyField(x => x.DiscountAmountWithTax));
            //Field<ListGraphType<CartShipmentItemType>>("items", resolve: context => context.Source.Items);
            //Field<MoneyType>("taxTotal", resolve: context => context.Source.TaxTotal);
            //Field(x => x.TaxPercentRate, nullable: true).Description("Tax percent rate");
            //Field(x => x.TaxType, nullable: true).Description("Tax type");
            //Field<ListGraphType<TaxDetailType>>("taxDetails", resolve: context => context.Source.TaxDetails);
            //Field(x => x.IsValid, nullable: true).Description("Is valid");
            //Field<ListGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.Source.ValidationErrors);
            //Field<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
            //Field<CurrencyType>("currency", resolve: context => context.Source.Currency);
            //.AddField<ListGraphType<ShipmentType>>("shipments", resolve: context => context.Source.Shipments);
            //.AddFieldAsync<ListGraphType<ShippingMethodType>>("availableShippingMethods", resolve: async context =>
            //{
            //    var shoppingCart = context.Source;
            //
            //    if (string.IsNullOrEmpty(shoppingCart.StoreId))
            //    {
            //        return Enumerable.Empty<ShippingMethod>();
            //    }
            //
            //    var criteria = new ShippingMethodsSearchCriteria
            //    {
            //        IsActive = true,
            //        Take = int.MaxValue,
            //        StoreId = shoppingCart.StoreId,
            //    };
            //
            //    var Shippings = await shippingMethodsSearchService.SearchShippingMethodsAsync(criteria);
            //
            //    var result = Shippings.Results.Select(x => x.ToShippingMethod(shoppingCart.Currency)).OrderBy(x => x.Priority).ToList();
            //
            //    if (shoppingCart != null && !result.IsNullOrEmpty() && !shoppingCart.IsTransient())
            //    {
            //        //Evaluate promotions cart and apply rewards for available shipping methods
            //        var promoEvalContext = shoppingCart.ToPromotionEvaluationContext();
            //        await promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);
            //
            //        // Load from
            //        var taxCalculationEnabled = context.UserContext.TryGetValue("taxCalculationEnabled", out var taxCalculationEnabledBoxed) && (bool)taxCalculationEnabledBoxed;
            //        var fixedTaxRate = context.UserContext.TryGetValue("fixedTaxRate", out var fixedTaxRateBoxed) ? (decimal)fixedTaxRateBoxed : 0M;
            //
            //        //Evaluate taxes for available Shippings
            //        var taxEvalContext = shoppingCart.ToTaxEvalContext(taxCalculationEnabled, fixedTaxRate);
            //        taxEvalContext.Lines.Clear();
            //        taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
            //        await taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
            //    }
            //
            //    return result;
            //});

            // Payment
            //.AddField<MoneyType>("paymentPrice", resolve: context => context.Source.PaymentPrice);
            //.AddField<MoneyType>("paymentPriceWithTax", resolve: context => context.Source.PaymentPriceWithTax);
            //.AddField<MoneyType>("paymentTotal", resolve: context => context.Source.PaymentTotal);
            //.AddField<MoneyType>("paymentTotalWithTax", resolve: context => context.Source.PaymentTotalWithTax);
            //.AddField<ListGraphType<PaymentType>>("payments", resolve: context => context.Source.Payments);
            //.AddFieldAsync<ListGraphType<PaymentMethodType>>("availablePaymentMethods", resolve: async context =>
            //{
            //    var shoppingCart = context.Source;
            //
            //    if (string.IsNullOrEmpty(shoppingCart.StoreId))
            //    {
            //        return Enumerable.Empty<PaymentMethod>();
            //    }
            //
            //    var criteria = new PaymentMethodsSearchCriteria
            //    {
            //        IsActive = true,
            //        Take = int.MaxValue,
            //        StoreId = shoppingCart.StoreId,
            //    };
            //
            //    var payments = await paymentMethodsSearchService.SearchPaymentMethodsAsync(criteria);
            //
            //    var result = payments.Results.Select(x => x.ToCartPaymentMethod(shoppingCart)).OrderBy(x => x.Priority).ToList();
            //
            //    if (shoppingCart != null && !result.IsNullOrEmpty() && !shoppingCart.IsTransient())
            //    {
            //        //Evaluate promotions cart and apply rewards for available shipping methods
            //        var promoEvalContext = shoppingCart.ToPromotionEvaluationContext();
            //        await promotionEvaluator.EvaluateDiscountsAsync(promoEvalContext, result);
            //
            //        // Load from
            //        var taxCalculationEnabled = context.UserContext.TryGetValue("taxCalculationEnabled", out var taxCalculationEnabledBoxed) && (bool)taxCalculationEnabledBoxed;
            //        var fixedTaxRate = context.UserContext.TryGetValue("fixedTaxRate", out var fixedTaxRateBoxed) ? (decimal)fixedTaxRateBoxed : 0M;
            //
            //        //Evaluate taxes for available payments
            //        var taxEvalContext = shoppingCart.ToTaxEvalContext(taxCalculationEnabled, fixedTaxRate);
            //        taxEvalContext.Lines.Clear();
            //        taxEvalContext.Lines.AddRange(result.SelectMany(x => x.ToTaxLines()));
            //        await taxEvaluator.EvaluateTaxesAsync(taxEvalContext, result);
            //    }
            //
            //    return result;
            //});
            //.AddField<ListGraphType<PaymentPlanType>>("paymentPlan", resolve: context => context.Source.PaymentPlan);

            //// Extended money
            //.AddField<MoneyType>("extendedPriceTotal", resolve: context => context.Source.ExtendedPriceTotal);
            //.AddField<MoneyType>("extendedPriceTotalWithTax", resolve: context => context.Source.ExtendedPriceTotalWithTax);

            //// Handling totals
            //.AddField<MoneyType>("handlingTotal", resolve: context => context.Source.HandlingTotal);
            //.AddField<MoneyType>("handlingTotalWithTax", resolve: context => context.Source.HandlingTotalWithTax);

            //// Discounts
            //.AddField<MoneyType>("discountTotal", resolve: context => context.Source.DiscountTotal);
            //.AddField<MoneyType>("discountTotalWithTax", resolve: context => context.Source.DiscountTotalWithTax);
            //.AddField<ListGraphType<DiscountType>>("discounts", resolve: context => context.Source.Discounts);
            //
            //// Addresses
            //.AddField<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Addresses);
            //
            //// Items
            //.AddField<ListGraphType<LineItemType>>("items", resolve: context => context.Source.Items);
            /*
            .AddField(x => x.ItemsCount)
            .AddField(x => x.ItemsQuantity)
            //.AddField<LineItemType>("recentlyAddedItem", resolve: context => context.Source.RecentlyAddedItem);
            //
            //// Coupon
            //.AddField<CopuponType>("coupon", resolve: context => context.Source.Coupon);
            //.AddField<ListGraphType<CopuponType>>("coupons", resolve: context => context.Source.Coupons);

            // Other
            .AddField(x => x.ObjectType)
            //.AddField<ListGraphType<DynamicPropertyType>>("dynamicProperties", resolve: context => context.Source.DynamicProperties); //todo add dynamic properties
            .AddField(x => x.IsValid)
            //.AddField<ListGraphType<ValidationErrorType>>("validationErrors", resolve: context => context.Source.ValidationErrors);
            .AddField(x => x.Type);*/

            return builder;
        }

    }
}
