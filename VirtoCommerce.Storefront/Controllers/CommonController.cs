using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Models;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class CommonController : StorefrontControllerBase
    {
        private readonly IStoreModule _storeApi;
        private readonly SignInManager<User> _signInManager;

        public CommonController(IWorkContextAccessor workContextAccesor, IStorefrontUrlBuilder urlBuilder, IStoreModule storeApi, SignInManager<User> signInManager)
            : base(workContextAccesor, urlBuilder)
        {
            _storeApi = storeApi;
            _signInManager = signInManager;
        }

        /// <summary>
        /// GET : /resetcache
        /// </summary>
        /// <returns></returns>
        [HttpGet("common/resetcache")]
        [Authorize(SecurityConstants.Permissions.CanResetCache)]
        public ActionResult ResetCache()
        {
            GlobalCacheRegion.ExpireRegion();

            return StoreFrontRedirect("~/");
        }

        /// <summary>
        /// POST : /contact
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        [HttpPost("contact/{viewName?}")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ContactForm([FromForm] ContactForm model, string viewName = "page.contact")
        {
            //TODO: Test with exist contact us form
            await _storeApi.SendDynamicNotificationAnStoreEmailAsync(model.ToServiceModel(WorkContext));
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
        [HttpGet("common/setcurrency/{currency}")]
        public async Task<ActionResult> SetCurrency(string currency, string returnUrl = "")
        {
            WorkContext.CurrentUser.SelectedCurrencyCode = currency;
            await _signInManager.RefreshSignInAsync(WorkContext.CurrentUser);
            //home page  and prevent open redirection attack
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "~/";
            }
            return StoreFrontRedirect(returnUrl);
        }

        // GET: common/getcountries/json
        [HttpGet("common/getcountries/json")]
        public ActionResult GetCountries()
        {
            return Json(WorkContext.AllCountries.Select(c => new Country { Name = c.Name, Code2 = c.Code2, Code3 = c.Code3, RegionType = c.RegionType })
                .ToArray());
        }

        // GET: common/getregions/{countryCode}/json
        [HttpGet("common/getregions/{countryCode}/json")]
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
        [HttpGet("common/maintenance")]
        [Route("maintenance")]
        public ActionResult Maintenance()
        {
            return View("Maintenance");
        }

        /// <summary>
        /// An internal special method for handling permanent redirection from routing rules
        /// </summary>
        /// <param name="url">URL to redirect</param>
        /// <returns>Redirect to URL</returns>
        public ActionResult InternalRedirect([FromRoute] string url)
        {
            return StoreFrontRedirectPermanent(url);
        }

        // GET: common/notheme
        [HttpGet("common/notheme")]
        public ActionResult NoTheme()
        {
            if (!HttpContext.Items.TryGetValue(NoLiquidThemeMiddleware.NoThemeModelKey, out var viewModel))
            {
                viewModel = new NoThemeViewModel();
            }

            return View("NoTheme", viewModel);
        }
    }
}
