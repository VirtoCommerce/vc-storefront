using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Recommendations
{
    public interface IRecommendationProviderFactory
    {
        IRecommendationsProvider GetProvider(string providerName);
    }
}
