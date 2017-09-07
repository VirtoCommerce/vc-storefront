using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Model.Recommendations
{
    public interface IRecommendationsService
    {
        string ProviderName { get; }
        Task<Product[]> GetRecommendationsAsync(RecommendationEvalContext context);
    }
}
