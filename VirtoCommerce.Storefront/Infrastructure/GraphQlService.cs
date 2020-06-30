using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Query.Builder;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Cart;
using VirtoCommerce.Storefront.Model.Commands;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class GraphQlService : IGraphQlService
    {
        private readonly IGraphQLClient _client;

        public GraphQlService(IGraphQLClient client)
        {
            _client = client;
        }

        public async Task<ShoppingCartDto> AddCouponAsync(string couponCode)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ShoppingCartDto> SearchShoppingCartAsync(Model.Cart.CartSearchCriteria criteria)
        {
            var builder = GetCartQuery(criteria);

            var query = "{" + builder.Build() + "}";

            var result = await _client.SendQueryAsync(
                new GraphQLRequest { Query = query },
                () => new
                {
                    Cart = new ShoppingCartDto()
                });

            return result.Data.Cart;
        }

        public async Task<ShoppingCartDto> AddItemToCartAsync(AddCartItemCommand command)
        {
            var request = new GraphQLRequest
            {
                Query = Mutation.AddItemToCart,
                Variables = new
                {
                    Command = command
                }
            };

            var result = await _client.SendMutationAsync(request, () => new { AddItem = new ShoppingCartDto() });

            return result.Data.AddItem;
        }

        private static IQuery<ShoppingCartDto> GetCartQuery(Model.Cart.CartSearchCriteria criteria)
        {
            var builder = new Query<ShoppingCartDto>("cart", new QueryOptions { Formatter = QueryFormatters.CamelCaseFormatter })
                .AddCartArguments(criteria)
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
                .AddTaxDetailsField(x => x.TaxDetails)

                // Shipping
                .AddMoneyField(x => x.ShippingPrice)
                .AddMoneyField(x => x.ShippingPriceWithTax)
                .AddMoneyField(x => x.ShippingTotal)
                .AddMoneyField(x => x.ShippingTotalWithTax)
                .AddShipmentsField(x => x.Shipments)
                .AddAvailableShippingMethodsField(x => x.AvailableShippingMethods)
                .AddDiscountsField(x => x.Discounts)
                .AddCurrencyField(x => x.Currency)


                // Payment
                .AddMoneyField(x => x.PaymentPrice)
                .AddMoneyField(x => x.PaymentPriceWithTax)
                .AddMoneyField(x => x.PaymentTotal)
                .AddMoneyField(x => x.PaymentTotalWithTax)
                .AddPaymentsField(x => x.Payments)
                .AddAvailablePaymentMethodsField(x => x.AvailablePaymentMethods)
                //.AddPaymentPlanField(x => x.PaymentPlan) //TODO: fix bug in CartType, r. 146

                // Extended money
                .AddMoneyField(x => x.ExtendedPriceTotal)
                .AddMoneyField(x => x.ExtendedPriceTotalWithTax)

                // Handling totals
                .AddMoneyField(x => x.HandlingTotal)
                .AddMoneyField(x => x.HandlingTotalWithTax)

                // Discounts
                .AddMoneyField(x => x.DiscountTotal)
                .AddMoneyField(x => x.DiscountTotalWithTax)

                // Addresses
                .AddAddressesField(x => x.Addresses)

                // Items
                .AddItemsField(x => x.Items)
                .AddLineItemField(x => x.RecentlyAddedItem)

                // Coupon
                .AddCouponField(x => x.Coupon)
                .AddCouponsField(x => x.Coupons)

                // Other
                .AddField(x => x.ObjectType)
                //.AddField<ListGraphType<DynamicPropertyType>>("dynamicProperties", resolve: context => context.Source.DynamicProperties); //todo add dynamic properties
                .AddField(x => x.IsValid)
                .AddValidationErrorsField(x => x.ValidationErrors)
                .AddField(x => x.Type);


            return builder;
        }
    }
}
