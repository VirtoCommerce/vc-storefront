using System.Collections.Generic;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Marketing.Services
{
    public interface IDynamicContentEvaluator
    {
        Task<IEnumerable<DynamicContentItem>> EvaluateDynamicContentItemsAsync(DynamicContentEvaluationContext context);
    }
}
