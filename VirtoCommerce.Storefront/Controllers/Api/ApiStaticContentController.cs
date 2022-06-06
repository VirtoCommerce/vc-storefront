using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

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

        // POST: storefrontapi/content/pages
        [HttpPost("pages")]
        public ActionResult FindPage([FromBody]ContentInThemeSearchCriteria value)
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
        public ActionResult LoadTemplate([FromBody]ContentInThemeSearchCriteria value)
        {
            var template = value.Template;
            var result = WorkContext.Templates.FirstOrDefault(x => x.Name == template);
            if (result == null)
            {
                return NotFound();
            }
            return JsonResult(result.Content);
        }

        private ActionResult JsonResult(string content)
        {
            return Content(content, "application/json");
        }
    }
}
