using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("theme")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiThemeController : StorefrontControllerBase
    {
        private readonly ILiquidThemeEngine _liquidThemeEngine;
        public ApiThemeController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder,  ILiquidThemeEngine liquidThemeEngine)
            : base(workContextAccessor, urlBuilder)
        {
            _liquidThemeEngine = liquidThemeEngine;
        }

        // GET: storefrontapi/theme/context
        [HttpGet("context")]
        public ActionResult<SpaThemeContext> GetSpaThemeContext()
        {
            var result = SpaThemeContext.Create(WorkContext, UrlBuilder);
            result.Settings = _liquidThemeEngine.GetSettings();
            return result;
        }
    }
}
