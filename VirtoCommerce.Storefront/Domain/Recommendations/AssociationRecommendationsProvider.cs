using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class AssociationRecommendationsProvider : IRecommendationsProvider
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICatalogService _catalogService;

        public AssociationRecommendationsProvider(IWorkContextAccessor workContextAccessor, ICatalogService catalogService)
        {
            _workContextAccessor = workContextAccessor;
            _catalogService = catalogService;
        }

        #region IRecommendationsService members
        public string ProviderName
        {
            get
            {
                return "Association";
            }
        }

        public RecommendationEvalContext CreateEvalContext()
        {
            return new RecommendationEvalContext();
        }

        public Task AddEventAsync(IEnumerable<UsageEvent> events)
        {
            //Nothing todo
            return Task.FromResult(true);
        }

        public async Task<Product[]> GetRecommendationsAsync(RecommendationEvalContext context)
        {
            var products = await _catalogService.GetProductsAsync(context.ProductIds.ToArray(), ItemResponseGroup.ItemInfo);
            var result = products.SelectMany(x => x.Associations).Take(context.Take).Select(x => x.Product).ToArray();
            return result;
        }
        #endregion

    }
}
