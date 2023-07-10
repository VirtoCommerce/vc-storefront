using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    [StorefrontApiRoute("")]
    [ResponseCache(CacheProfileName = "None")]
    public class ApiCommonController : StorefrontControllerBase
    {
        private readonly IStoreModule _storeApi;
        private readonly ISeoInfoService _seoInfoService;
        private readonly Country[] _countriesWithoutRegions;

        public ApiCommonController(
            IWorkContextAccessor workContextAccessor,
            IStorefrontUrlBuilder urlBuilder,
            IStoreModule storeApi,
            ISeoInfoService seoInfoService) : base(workContextAccessor, urlBuilder)
        {
            _storeApi = storeApi;
            _seoInfoService = seoInfoService;
            _countriesWithoutRegions = WorkContext.AllCountries
                 .Select(c => new Country { Name = c.Name, Code2 = c.Code2, Code3 = c.Code3, RegionType = c.RegionType })
                 .ToArray();
        }

        // GET: storefrontapi/countries
        [HttpGet("countries")]
        public ActionResult<Country[]> GetCountries()
        {
            return _countriesWithoutRegions;
        }

        // GET: storefrontapi/countries/{countryCode}/regions
        [HttpGet("countries/{countryCode}/regions")]
        public ActionResult<CountryRegion[]> GetCountryRegions(string countryCode)
        {
            var country = WorkContext.AllCountries.FirstOrDefault(c => c.Code2.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase) || c.Code3.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase));
            if (country != null)
            {
                return country.Regions;
            }
            return Ok();
        }

        // POST: storefrontapi/feedback
        [HttpPost("feedback")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Feedback([FromBody] ContactForm model)
        {
            await _storeApi.SendDynamicNotificationAnStoreEmailAsync(model.ToServiceModel(WorkContext));

            return Ok();
        }

        [HttpGet("slug/{*slug}")]
        public async Task<SlugInfoResult> GetInfoBySlugAsync(string slug, [FromQuery] string culture)
        {
            var result = new SlugInfoResult();

            if (string.IsNullOrEmpty(slug))
            {
                return result;
            }

            var segments = slug.Split("/", StringSplitOptions.RemoveEmptyEntries);
            var lastSegment = segments.Last();
            var seoInfos = await _seoInfoService.GetBestMatchingSeoInfos(lastSegment, WorkContext.CurrentStore, culture);
            var bestSeoInfo = seoInfos.FirstOrDefault();
            result.EntityInfo = bestSeoInfo;

            if (result.EntityInfo == null)
            {
                var pageUrl = slug == "__index__home__page__" ? "/" : $"/{slug}";
                try
                {
                    var pages = WorkContext.Pages.Where(p =>
                        string.Equals(p.Url, pageUrl, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(p.Url, slug, StringComparison.OrdinalIgnoreCase)
                    );

                    var page = pages.FirstOrDefault(x => x.Language.CultureName.EqualsInvariant(culture))
                               ?? pages.FirstOrDefault(x => x.Language.IsInvariant)
                               ?? pages.FirstOrDefault(x => x.AliasesUrls.Contains(pageUrl, StringComparer.OrdinalIgnoreCase));
                    result.ContentItem = page;

                }
                catch
                {
                    //do nothing
                }
            }

            return result;
        }

        // GET: storefrontapi/version
        [HttpGet("version")]
        [AllowAnonymous]
        public ActionResult Version()
        {
            return Ok(System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location).FileVersion);
        }
    }
}
