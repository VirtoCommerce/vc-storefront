using System.Linq;
using VirtoCommerce.Storefront.Model.Recommendations;
using dto = VirtoCommerce.Storefront.AutoRestClients.ProductRecommendationsModuleApi.Models;

namespace VirtoCommerce.Storefront.Domain
{
    public static class CognitiveRecommendationConverterExtension
    {
        public static CognitiveRecommendationConverter RecommendationConverterInstance
        {
            get
            {
                return new CognitiveRecommendationConverter();
            }
        }

        public static dto.RecommendationEvalContext ToContextDto(this CognitiveRecommendationEvalContext context)
        {
            return RecommendationConverterInstance.ToContextDto(context);
        }    
    }

    public partial class CognitiveRecommendationConverter
    {
        public virtual dto.RecommendationEvalContext ToContextDto(CognitiveRecommendationEvalContext context)
        {
            var retVal = new dto.RecommendationEvalContext
            {
                BuildId = context.BuildId,
                ModelId = context.ModelId,
                ProductIds = context.ProductIds.Where(x => !string.IsNullOrEmpty(x)).ToList(),
                StoreId = context.StoreId,
                Take = context.Take,
                Type = context.Type,
                UserId = context.UserId
            };

            return retVal;
        }
    }
}
