namespace VirtoCommerce.Storefront.Model.Recommendations
{
    public interface IRecommendationProviderFactory
    {
        IRecommendationsProvider GetProvider(string providerName);
    }
}
