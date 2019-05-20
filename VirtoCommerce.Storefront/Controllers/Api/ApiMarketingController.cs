using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Marketing;
using VirtoCommerce.Storefront.Model.Marketing.Services;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("marketing")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiMarketingController : StorefrontControllerBase
    {
        private readonly IDynamicContentEvaluator _dynamicContentEvaluator;
        public ApiMarketingController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IDynamicContentEvaluator dynamicContentEvaluator)
            : base(workContextAccessor, urlBuilder)
        {
            _dynamicContentEvaluator = dynamicContentEvaluator;
        }

        // GET: storefrontapi/marketing/dynamiccontent/{placeName}
        [HttpGet("dynamiccontent/{placeName}")]
        public async Task<ActionResult<string>> GetDynamicContent(string placeName)
        {
            string htmlContent = null;

            var evalContext = WorkContext.ToDynamicContentEvaluationContext();
            evalContext.PlaceName = placeName;
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
