using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Recommendations;
using VirtoCommerce.Storefront.Model.Services;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi;
using dto = VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi.Models;
using VirtoCommerce.Storefront.Common;

namespace VirtoCommerce.Storefront.Services.Recommendations
{
    public class AssociationRecommendationsService : IRecommendationsService
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly ICatalogService _catalogService;
        private readonly IRecommendations _recommendationApi;

        public AssociationRecommendationsService(IWorkContextAccessor workContextAccessor, ICatalogService catalogService, IRecommendations recommendationApi)
        {
            _workContextAccessor = workContextAccessor;
            _catalogService = catalogService;
            _recommendationApi = recommendationApi;
        }

        #region IRecommendationsService members
        public string ProviderName
        {
            get
            {
                return "Association";
            }
        }

        public async Task AddEventAsync(IEnumerable<UsageEvent> events)
        {
            var usageEvents = events.Select(i => i.JsonConvert<dto.UsageEvent>());

            await _recommendationApi.AddEventAsync(usageEvents.ToList());
        }

        public async Task<Product[]> GetRecommendationsAsync(RecommendationEvalContext context)
        {
            Product[] products = await _catalogService.GetProductsAsync(context.ProductIds.ToArray(), ItemResponseGroup.ItemAssociations);

            //Need to load related products from associated product and categories
            var retVal = products.SelectMany(p => p.Associations.OfType<ProductAssociation>().OrderBy(x => x.Priority))
                                 .Select(a => a.Product).ToList();
            retVal.AddRange(products.SelectMany(p => p.Associations.OfType<CategoryAssociation>().OrderBy(x => x.Priority))
                                .SelectMany(a => a.Category.Products.ToArray()));

            return retVal.Take(context.Take).ToArray();
        } 
        #endregion

    }
}