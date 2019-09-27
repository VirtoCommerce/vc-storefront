using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Marketing.Services;
using VirtoCommerce.Storefront.Model.Services;

namespace VirtoCommerce.Storefront.Domain
{
    public class MarketingService : IMarketingService
    {
        private readonly IDynamicContentEvaluator _dynamicContentEvaluator;
        private readonly IWorkContextAccessor _workContextAccessor;

        public MarketingService(IDynamicContentEvaluator dynamicContentEvaluator, IWorkContextAccessor workContextAccessor)
        {
            _dynamicContentEvaluator = dynamicContentEvaluator;
            _workContextAccessor = workContextAccessor;
        }

        [Obsolete("Use IDynamicContentEvaluator instead")]
        public virtual async Task<string> GetDynamicContentHtmlAsync(string storeId, string placeholderName)
        {
            string htmlContent = null;

            var evalContext = _workContextAccessor.WorkContext.ToDynamicContentEvaluationContext();
            evalContext.PlaceName = placeholderName;
            evalContext.StoreId = storeId;
            var result = await _dynamicContentEvaluator.EvaluateDynamicContentItemsAsync(evalContext);
            if (result != null)
            {
                var htmlContentSpec = new HtmlDynamicContentSpecification();
                var htmlDynamicContent = result.FirstOrDefault(htmlContentSpec.IsSatisfiedBy);
                if (htmlDynamicContent != null)
                {
                    var dynamicProperty = htmlDynamicContent.DynamicProperties.FirstOrDefault(htmlContentSpec.IsSatisfiedBy);
                    if (dynamicProperty != null && dynamicProperty.Values.Any(v => v.Value != null))
                    {
                        htmlContent = dynamicProperty.Values.First().Value.ToString();
                    }
                }
            }
            return htmlContent;
        }
    }
}
