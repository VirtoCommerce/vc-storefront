using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Customer;
using VirtoCommerce.Storefront.Models;

namespace VirtoCommerce.Storefront.Controllers
{
    public class CommonController : StorefrontControllerBase
    {
        private readonly IStoreModule _storeApi;
        private readonly ISecurity _platformSecurityApi;
        private readonly SignInManager<CustomerInfo> _signInManager;
        public CommonController(IWorkContextAccessor workContextAccesor, IStorefrontUrlBuilder urlBuilder, IStoreModule storeApi,
                                 ISecurity platformSecurityApi, SignInManager<CustomerInfo> signInManager)
              : base(workContextAccesor, urlBuilder)
        {
            _storeApi = storeApi;
            _platformSecurityApi = platformSecurityApi;
            _signInManager = signInManager;
        }

        /// <summary>
        /// POST : /resetcache
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ResetCache()
        {
            //check permissions
            if (_platformSecurityApi.UserHasAnyPermission(WorkContext.CurrentCustomer.UserName, new[] { "cache:reset" }.ToList(), new List<string>()).Result ?? false)
            {
               //TODO: Reset global cache
                return StoreFrontRedirect("~/");
            }
            return Unauthorized();
        }

        /// <summary>
        /// POST : /contact
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> СontactForm([FromForm]ContactForm model, string viewName = "page.contact")
        {
            //TODO: Test with exist contact us form 
            await _storeApi.SendDynamicNotificationAnStoreEmailAsync(model.ToServiceModel(WorkContext));
            WorkContext.ContactUsForm = model;
            if (model.Contact.ContainsKey("RedirectUrl") && model.Contact["RedirectUrl"].Any())
            {
                return StoreFrontRedirect(model.Contact["RedirectUrl"].First());
            }
            return View(viewName, WorkContext);
        }

        /// <summary>
        /// GET: common/setcurrency/{currency}
        /// Set current currency for current user session
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> SetCurrency(string currency, string returnUrl = "")
        {
            WorkContext.CurrentCustomer.SelectedCurrencyCode = currency;            
            await _signInManager.RefreshSignInAsync(WorkContext.CurrentCustomer);
            //home page  and prevent open redirection attack
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "~/";
            }
            return StoreFrontRedirect(returnUrl);
        }

        // GET: common/getcountries/json
        [HttpGet]
        public ActionResult GetCountries()
        {
            return Json(WorkContext.AllCountries.Select(c => new Country { Name = c.Name, Code2 = c.Code2, Code3 = c.Code3, RegionType = c.RegionType })
                .ToArray());
        }

        // GET: common/getregions/{countryCode}/json
        [HttpGet]
        public ActionResult GetRegions(string countryCode)
        {
            Country country = null;

            if (countryCode != null)
            {
                if (countryCode.Length == 3)
                {
                    country = WorkContext.AllCountries.FirstOrDefault(c => c.Code3.EqualsInvariant(countryCode));
                }
                else if (countryCode.Length == 2)
                {
                    country = WorkContext.AllCountries.FirstOrDefault(c => c.Code2.EqualsInvariant(countryCode));
                }
            }
            if (country != null)
            {
                return Json(country.Regions);
            }

            return NotFound();
        }

        // GET: common/maintenance
        [HttpGet]
        public ActionResult Maintenance()
        {
            return View("Maintenance");
        }

        // GET: common/notheme
        [HttpGet]
        public ActionResult NoTheme()
        {
            object viewModel;
            if(!HttpContext.Items.TryGetValue(NoLiquidThemeMiddleware.NoThemeModelKey, out viewModel))
            {
                viewModel = new NoThemeViewModel();
            }
            return View("NoTheme", viewModel);
        }

    }
}
