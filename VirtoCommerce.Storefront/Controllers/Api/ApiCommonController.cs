using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers.Api
{
    public class ApiCommonController : StorefrontControllerBase
    {
        private readonly IStoreModule _storeApi;
        private readonly Country[] _countriesWithoutRegions;

        public ApiCommonController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IStoreModule storeApi)
            : base(workContextAccessor, urlBuilder)
        {
            _storeApi = storeApi;
            _countriesWithoutRegions = WorkContext.AllCountries
             .Select(c => new Country { Name = c.Name, Code2 = c.Code2, Code3 = c.Code3, RegionType = c.RegionType })
             .ToArray();
        }

        // GET: storefrontapi/countries
        [HttpGet]
        public ActionResult GetCountries()
        {
            return Json(_countriesWithoutRegions);
        }

        // GET: storefrontapi/countries/{countryCode}/regions
        [HttpGet]
        public ActionResult GetCountryRegions(string countryCode)
        {
            var country = WorkContext.AllCountries.FirstOrDefault(c => c.Code2.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase) || c.Code3.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase));
            if (country != null)
            {
                return Json(country.Regions);
            }
            return Ok();
        }

        // POST: storefrontapi/feedback
        [HttpPost]
        public async Task<ActionResult> Feedback([FromBody] ContactForm model)
        {
            await _storeApi.SendDynamicNotificationAnStoreEmailAsync(model.ToServiceModel(WorkContext));

            return Ok();
        }
    }
}
