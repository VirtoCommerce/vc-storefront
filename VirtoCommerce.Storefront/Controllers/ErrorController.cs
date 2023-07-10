using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute("error")]
    public class ErrorController : StorefrontControllerBase
    {
        private readonly ISeoInfoService _seoInfoService;
        private readonly ISpaRouteService _spaRouteService;

        public ErrorController(
            IWorkContextAccessor workContextAccessor,
            IStorefrontUrlBuilder urlBuilder,
            ISeoInfoService seoInfoService,
            ISpaRouteService spaRouteService)
            : base(workContextAccessor, urlBuilder)
        {
            _seoInfoService = seoInfoService;
            _spaRouteService = spaRouteService;
        }

        [Route("{errCode}")]
        public async Task<IActionResult> Error(int? errCode)
        {
            //Returns index page on 404 error when the store.IsSpa flag is activated 
            if (errCode == StatusCodes.Status404NotFound && WorkContext.CurrentStore.IsSpa)
            {
                var path = TrimTwoLetterLangSegment(Request.HttpContext.Features.Get<IStatusCodeReExecuteFeature>()?.OriginalPath);
                Response.StatusCode = StatusCodes.Status404NotFound;

                if (path == "/")
                {
                    Response.StatusCode = StatusCodes.Status200OK;
                    return View("index");
                }

                if (string.IsNullOrEmpty(path))
                {
                    return View("index");
                }

                var slug = path.Split('/').Last();
                if (!string.IsNullOrEmpty(slug))
                {
                    var seoInfos = await _seoInfoService.GetBestMatchingSeoInfos(slug, WorkContext.CurrentStore, WorkContext.CurrentLanguage.CultureName);
                    Response.StatusCode = seoInfos.Any() ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
                }

                if (Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    Response.StatusCode = _seoInfoService.GetContentItem($"/{slug}", WorkContext) != null ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
                }

                if (Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    Response.StatusCode = await _spaRouteService.IsSpaRoute(path) ? StatusCodes.Status200OK : StatusCodes.Status404NotFound;
                }

                return View("index");
            }
            var exceptionFeature = base.HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature != null && exceptionFeature.Error is StorefrontException storefrontException && storefrontException.View != null)
            {
                return View(storefrontException.View);
            }
            if (errCode == StatusCodes.Status404NotFound || errCode == StatusCodes.Status500InternalServerError)
            {
                return View(errCode.ToString());
            }
            return View();
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return View("AccessDenied");
        }

        private string TrimTwoLetterLangSegment(string path)
        {
            var language = WorkContext.CurrentStore.Languages.FirstOrDefault(x => Regex.IsMatch(path, @"^/\b" + x.TwoLetterLanguageName + @"\b/", RegexOptions.IgnoreCase));

            if (language != null)
            {
                path = Regex.Replace(path, @"/\b" + language.TwoLetterLanguageName + @"\b/", "/", RegexOptions.IgnoreCase);
            }

            return path;
        }
    }
}
