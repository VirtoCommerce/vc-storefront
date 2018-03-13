using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Domain.Lists;
using VirtoCommerce.Storefront.Domain.Security;
using VirtoCommerce.Storefront.Middleware;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Models;

namespace VirtoCommerce.Storefront.Controllers
{
    public class CommonController : StorefrontControllerBase
    {
        private readonly IStoreModule _storeApi;
        private readonly SignInManager<User> _signInManager;
        public CommonController(IWorkContextAccessor workContextAccesor, IStorefrontUrlBuilder urlBuilder, IStoreModule storeApi,
                                 ISecurity platformSecurityApi, SignInManager<User> signInManager)
              : base(workContextAccesor, urlBuilder)
        {
            _storeApi = storeApi;
            _signInManager = signInManager;
        }

        /// <summary>
        /// GET : /resetcache
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = "CanResetCache")]
        public ActionResult ResetCache()
        {

            //TODO: Replace to some other (maybe with using reflection)
            ThemeEngineCacheRegion.ExpireRegion();
            CartCacheRegion.ExpireRegion();
            CatalogCacheRegion.ExpireRegion();
            ContentBlobCacheRegion.ExpireRegion();
            CustomerCacheRegion.ExpireRegion();
            MarketingCacheRegion.ExpireRegion();
            PricingCacheRegion.ExpireRegion();
            QuoteCacheRegion.ExpireRegion();
            RecommendationsCacheRegion.ExpireRegion();
            StaticContentCacheRegion.ExpireRegion();
            StoreCacheRegion.ExpireRegion();
            TaxCacheRegion.ExpireRegion();
            SubscriptionCacheRegion.ExpireRegion();
            SecurityCacheRegion.ExpireRegion();
            InventoryCacheRegion.ExpireRegion();
            WishlistCacheRegion.ExpireRegion();

            return StoreFrontRedirect("~/");
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

        //An internal special method for handling permanent redirection from routing rules
        public ActionResult InternalRedirect([FromRoute] string url)
        {
            return RedirectPermanent(url);
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
