using System.Text;
using System.Threading.Tasks;
using AutoRest.Core.Utilities;
using GraphQL;
using GraphQL.Client.Abstractions;
using Newtonsoft.Json;
using PagedList.Core;
using VirtoCommerce.Storefront.Extensions;
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
            var request = new GraphQLRequest
            {
                Query = this.SearchOrdersRequest(criteria.Sort,
                                                PrepareFilter(criteria),
                                                _workContextAccessor.WorkContext.CurrentLanguage.CultureName,
                                                criteria.PageSize,
                                                (criteria.PageNumber - 1) * criteria.PageSize)
            };
            var searchResult = await _client.SendQueryAsync<OrdersResponseDto>(request);

            return new StaticPagedList<CustomerOrder>(searchResult.Data?.Orders.Items, criteria.PageNumber, criteria.PageSize, searchResult.Data.Orders.TotalCount);
        }

        public async Task<CustomerOrder> CreateOrderFromCartAsync(string cartId)
        {
            var request = new GraphQLRequest
            {
                Query = this.CreateOrderRequest(),
                Variables = new
                {
                    Command = new { cartId }
                }
            };
            var response = await _client.SendMutationAsync<OrderResponseDto>(request);
            response.ThrowExceptionOnError();

            return response.Data?.Order;
        }

        public async Task UpdateOrderAsync(CustomerOrder order)
        {
            var request = new GraphQLRequest
            {
                Query = this.UpdateOrderRequest(),
                Variables = new
                {
                    Command = order.ToCustomerOrderDto()
                }
            };
            var response = await _client.SendMutationAsync<UpdateOrderResponseDto>(request);
            response.ThrowExceptionOnError();
        }

        public async Task ChangeOrderStatusAsync(string orderId, string status)
        {
            var request = new GraphQLRequest
            {
                Query = this.ChangeOrderStatusRequest(),
                Variables = new
                {
                    Command = new { orderId, status }
                }
            };
            var response = await _client.SendMutationAsync<ChangeOrderStatusResponseDto>(request);
            response.ThrowExceptionOnError();
        }

        public async Task ConfirmPayment(PaymentIn payment)
        {
            var request = new GraphQLRequest
            {
                Query = this.ConfirmOrderPaymentRequest(),
                Variables = new
                {
                    Command = new { payment = payment.ToOrderPaymentInDto() }
                }
            };

            var response = await _client.SendMutationAsync<object>(request);
            response.ThrowExceptionOnError();
        }

        public async Task CancelPayment(PaymentIn payment)
        {
            var request = new GraphQLRequest
            {
                Query = this.CancelOrderPaymentRequest(),
                Variables = new
                {
                    Command = new { payment = payment.ToOrderPaymentInDto() }
                }
            };

            var response = await _client.SendMutationAsync<object>(request);
            response.ThrowExceptionOnError();
        }
                

        //TODO more useful 
        private string PrepareFilter(OrderSearchCriteria criteria)
        {
            var filer = new StringBuilder();
            filer.Append(!string.IsNullOrEmpty(criteria.CustomerId) ? $"{nameof(OrderSearchCriteria.CustomerId).ToCamelCase()}:{criteria.CustomerId}" : string.Empty);
            filer.Append(!string.IsNullOrEmpty(criteria.Status) ? $"{nameof(OrderSearchCriteria.Status).ToCamelCase()}:{criteria.Status}" : string.Empty);
            filer.Append(!criteria.Statuses.IsNullOrEmpty() ? $"{nameof(OrderSearchCriteria.Statuses).ToCamelCase()}:{criteria.Statuses}" : string.Empty);
            filer.Append(!criteria.StoreIds.IsNullOrEmpty() ? $"{nameof(OrderSearchCriteria.StoreIds).ToCamelCase()}:{criteria.StoreIds}" : string.Empty);
            filer.Append(!string.IsNullOrEmpty(criteria.Keyword) ? $"{nameof(OrderSearchCriteria.Keyword).ToCamelCase()}:{criteria.Keyword}" : string.Empty);

            return filer.ToString();
        }        
    }
}
