using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Recommendations;

namespace VirtoCommerce.Storefront.Domain
{
    public class RecommendationProviderFactory : IRecommendationProviderFactory
    {
        private readonly IList<IRecommendationsProvider> _providers;
        public RecommendationProviderFactory(params IRecommendationsProvider[] providers)
        {
            _providers = providers;
        }
        public IRecommendationsProvider GetProvider(string providerName)
        {
            return _providers.FirstOrDefault(x => x.ProviderName.EqualsInvariant(providerName));
        }  
    }
}
