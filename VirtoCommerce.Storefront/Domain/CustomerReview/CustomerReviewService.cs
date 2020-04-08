using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using PagedList.Core;
using VirtoCommerce.Storefront.AutoRestClients.CustomerReviewsModuleModuleApi;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Domain.CustomerReview;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.CustomerReviews;

namespace VirtoCommerce.Storefront.Domain
{
    public class CustomerReviewService : ICustomerReviewService
    {
        private readonly ICustomerReviewsModule _customerReviewsApi;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IApiChangesWatcher _apiChangesWatcher;

        public CustomerReviewService(ICustomerReviewsModule customerReviewsApi, IStorefrontMemoryCache memoryCache, IApiChangesWatcher apiChangesWatcher)
        {
            _customerReviewsApi = customerReviewsApi;
            _memoryCache = memoryCache;
            _apiChangesWatcher = apiChangesWatcher;
        }

        public async Task CreateReviewAsync(CreateCustomerReviewRequest request, ItemResponseGroup responseGroup)
        {
            await _customerReviewsApi.PublishNewReviewAsync(request.ToApiModel());
        }

        public IPagedList<Model.CustomerReviews.CustomerReview> SearchReviews(CustomerReviewSearchCriteria criteria)
        {
            return SearchReviewsAsync(criteria).GetAwaiter().GetResult();
        }

        public async Task<IPagedList<Model.CustomerReviews.CustomerReview>> SearchReviewsAsync(CustomerReviewSearchCriteria criteria)
        {
            var result = await _customerReviewsApi.SearchCustomerReviewsAsync(criteria.ToSearchCriteriaDto());
            return new StaticPagedList<Model.CustomerReviews.CustomerReview>(result.Results.Select(x => x.ToCustomerReview()),
                                                        criteria.PageNumber, criteria.PageSize, result.TotalCount.Value);
        }
    }
}
