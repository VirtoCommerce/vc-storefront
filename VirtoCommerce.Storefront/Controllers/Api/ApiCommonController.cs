using System;
using System.Linq;
using System.Threading.Tasks;
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

        public ApiCommonController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IStoreModule storeApi, ISeoInfoService seoInfoService)
            : base(workContextAccessor, urlBuilder)
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

        [HttpGet("seoInfos/{slug}")]
        public async Task<SeoInfo[]> GetSeoInfosAsync(string slug)
        {
            return await _seoInfoService.GetSeoInfosBySlug(slug);
        }
    }
}
