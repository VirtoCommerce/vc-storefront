using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Models;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("content")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiStaticContentController : StorefrontControllerBase
    {
        public ApiStaticContentController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder)
            : base(workContextAccessor, urlBuilder)
        {
        }

        // POST: storefrontapi/content/reset-cache
        [HttpPost("reset-cache")]
        public ActionResult ResetCache([FromBody] ResetCacheEventModel webHookEvent)
        {
            if (TryResetCacheInternal(webHookEvent?.EventBody?.FirstOrDefault()?.Type))
            {
                return Ok("OK");
            }
            // we can't return 400, because webhook module use it to repeat request
            return Ok("Failed");
        }

        // POST: storefrontapi/content/reset-cache/theme
        [HttpPost("reset-cache/{region}")]
        public ActionResult ResetCacheRegion([FromRoute] string region)
        {
            if (TryResetCacheInternal(region))
            {
                return Ok("OK");
            }
            // we can't return 400, because webhook module use it to repeat request
            return Ok("Failed");
        }

        private static bool TryResetCacheInternal(string region)
        {
            switch (region)
            {
                case "theme":
                case "themes":
                    ThemeEngineCacheRegion.ExpireRegion();
                    return true;
                case "pages":
                case "blogs":
                    StaticContentCacheRegion.ExpireRegion();
                    return true;
            }
            return false;
        }

        // POST: storefrontapi/content/pages
        [HttpPost("pages")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult FindPage([FromBody] ContentInThemeSearchCriteria value)
        {
            var permalink = value.Permalink;
            var result = WorkContext.Pages.FirstOrDefault(x => x.Permalink != null && x.Permalink.EqualsInvariant(permalink));
            if (result == null)
            {
                return NotFound();
            }
            return JsonResult(result.Content);
        }

        [HttpPost("templates")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult LoadTemplate([FromBody] ContentInThemeSearchCriteria value)
        {
            var result = WorkContext.Templates.FirstOrDefault(x => x.Name == value.TemplateName);
            if (result == null)
            {
                return NotFound();
            }
            return JsonResult(result.Content);
        }

        private ActionResult JsonResult(string content)
        {
            return Content(content, MediaTypeNames.Application.Json);
        }
    }
}
