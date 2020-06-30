using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Abstractions;
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

        public async Task<ShoppingCartDto> AddCouponAsync(AddCouponCommand command)
        {
            var request = new GraphQLRequest
            {
                Query = QueryHelper.AddCoupon(QueryHelper.AllFields()),
                Variables = new
                {
                    Command = command
                }
            };

            var result = await _client.SendMutationAsync(request, () => new { AddItem = new ShoppingCartDto() });

            return result.Data.AddItem;
        }

        public async Task<ShoppingCartDto> SearchShoppingCartAsync(Model.Cart.CartSearchCriteria criteria)
        {
            var query = QueryHelper.GetCart(
                storeId: criteria.StoreId,
                cartName: criteria.Name,
                userId: criteria.Customer?.Id,
                cultureName: criteria.Language?.CultureName ?? "en-US",
                currencyCode: criteria.Currency.Code,
                type: criteria.Type ?? string.Empty,
                selectedFields: QueryHelper.AllFields());

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
                Query = QueryHelper.AddItemToCart(QueryHelper.AllFields()),
                Variables = new
                {
                    Command = command
                }
            };

            var result = await _client.SendMutationAsync(request, () => new { AddItem = new ShoppingCartDto() });

            return result.Data.AddItem;
        }

        //private async Task<T> SendMutationAsync<T>(string query, object command, Func<T> responseStructure)
        //{
        //    var request = new GraphQLRequest
        //    {
        //        Query = query,
        //        Variables = new
        //        {
        //            Command = command
        //        }
        //    };

        //    var result = await _client.SendMutationAsync(request, responseStructure);

        //    return responseStructure();
        //}
    }
}
