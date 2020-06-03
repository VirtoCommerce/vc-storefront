using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.AutoRestClients.SitemapsModuleApi;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Controllers
{
    [StorefrontRoute]
    public class SitemapController : StorefrontControllerBase
    {
        private readonly ISitemapsModuleApiOperations _siteMapApi;
        private readonly ILiquidThemeEngine _liquidThemeEngine;
        public SitemapController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, ISitemapsModuleApiOperations siteMapApi,
                                 ILiquidThemeEngine themeEngine)
            : base(workContextAccessor, urlBuilder)
        {
            _liquidThemeEngine = themeEngine;
            _siteMapApi = siteMapApi;
        }

        /// <summary>
        /// GET: /sitemap.xml
        /// Return generated sitemap index sitemap.xml
        /// </summary>
        /// <returns></returns>
        [HttpGet("sitemap.xml")]
        public async Task<ActionResult> GetSitemapIndex()
        {
            var stream = await TryGetSitemapStream("sitemap.xml");
            if (stream != null)
            {
                return File(stream, "text/xml");
            }
            return NotFound("sitemap.xml");
        }

        /// <summary>
        /// GET: /sitemap/sitemap-1.xml
        /// Return generated sitemap by file
        /// </summary>
        /// <returns></returns>
        [HttpGet("sitemap/{sitemapPath}")]
        public async Task<ActionResult> GetSitemap(string sitemapPath)
        {
            var stream = await TryGetSitemapStream("sitemap/" + sitemapPath);
            if (stream != null)
            {
                return File(stream, "text/xml");
            }
            return NotFound(sitemapPath);
        }


        private async Task<Stream> TryGetSitemapStream(string filePath)
        {
            //If sitemap files have big size for generation on the fly you might place already generated xml files in the theme/assets folder or schedule 
            // execution of GenerateSitemapJob.GenerateStoreSitemap method for pre-generation sitemaps  
            var stream = await _liquidThemeEngine.GetAssetStreamAsync(filePath);
            if (stream == null)
            {
                var absUrl = UrlBuilder.ToAppAbsolute("~/", WorkContext.CurrentStore, WorkContext.CurrentLanguage);
                var storeUrl = new Uri(WorkContext.RequestUrl, absUrl).ToString();
                //remove language from base url SitemapAPI will add it automatically
                storeUrl = storeUrl.Replace("/" + WorkContext.CurrentLanguage.CultureName + "/", "/");
                stream = await _siteMapApi.GenerateSitemapAsync(WorkContext.CurrentStore.Id, storeUrl, filePath);
            }
            return stream;
        }

    }
}
