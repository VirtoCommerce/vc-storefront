using System.Text;
using System.Threading.Tasks;
using AutoRest.Core.Utilities;
using GraphQL;
using GraphQL.Client.Abstractions;
using PagedList.Core;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Order;
using VirtoCommerce.Storefront.Model.Order.Contracts;
using VirtoCommerce.Storefront.Model.Order.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class ExperienceCustomerOrderService : ICustomerOrderService
    {
        private readonly IGraphQLClient _client;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ExperienceCustomerOrderService(IGraphQLClient client, IWorkContextAccessor workContextAccessor)
        {
            _client = client;
            _workContextAccessor = workContextAccessor;
        }

        public async Task<CustomerOrder> GetOrderByIdAsync(string id)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetOrderByIdRequest(id)
            };
            var response = await _client.SendQueryAsync<OrderResponseDto>(request);

            return response.Data?.Order;
        }

        public async Task<CustomerOrder> GetOrderByNumberAsync(string number)
        {
            var request = new GraphQLRequest
            {
                Query = this.GetOrderByNumberRequest(number)
            };
            var response = await _client.SendQueryAsync<OrderResponseDto>(request);

            return response.Data?.Order;
        }

        public IPagedList<CustomerOrder> SearchOrders(OrderSearchCriteria criteria)
        {
            return SearchOrdersAsync(criteria).GetAwaiter().GetResult();
        }

        public async Task<IPagedList<CustomerOrder>> SearchOrdersAsync(OrderSearchCriteria criteria)
        {
            var filer = new StringBuilder();
            filer.Append(!string.IsNullOrEmpty(criteria.CustomerId) ? $"{nameof(OrderSearchCriteria.CustomerId).ToCamelCase()}:{criteria.CustomerId}" : string.Empty);

            var request = new GraphQLRequest
            {
                Query = this.SearchOrdersRequest(criteria.Sort, filer.ToString(), criteria.PageSize, (criteria.PageNumber - 1) * criteria.PageSize)
            };
            var searchResult = await _client.SendQueryAsync<SearchOrdersResponseDto>(request);

            return new PagedList<CustomerOrder>(searchResult.Data.Items, criteria.PageNumber, criteria.PageSize);
        }
    }
}
