using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers
{
    public class StorefrontControllerBase : Controller
    {
        protected IStorefrontUrlBuilder UrlBuilder { get; }

        protected WorkContext WorkContext { get; }

        public StorefrontControllerBase(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder)
        {
            WorkContext = workContextAccessor.WorkContext;
            UrlBuilder = urlBuilder;
        }

        protected RedirectResult StoreFrontRedirect(string url)
        {
            var newUrl = Url.IsLocalUrl(url) ? url : "~/";
            var appRelativeUrl = UrlBuilder.ToAppRelative(newUrl, WorkContext.CurrentStore, WorkContext.CurrentLanguage);

            return base.Redirect(appRelativeUrl);
        }
		
		protected RedirectResult StoreFrontRedirectPermanent(string url)
        {
            var newUrl = Url.IsLocalUrl(url) ? url : "~/";
            var appRelativeUrl = UrlBuilder.ToAppRelative(newUrl, WorkContext.CurrentStore, WorkContext.CurrentLanguage);

            return base.RedirectPermanent(appRelativeUrl);
        }
    }
}
