using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public class SeoInfoService : ISeoInfoService
    {
        private readonly ICommerce _coreModuleApi;

        public SeoInfoService(ICommerce coreModuleApi)
        {
            _coreModuleApi = coreModuleApi;
        }

        public async Task<SeoInfo[]> GetSeoInfosBySlug(string slug)
        {
            var result = (await _coreModuleApi.GetSeoInfoBySlugAsync(slug)).Select(x => x.ToSeoInfo()).ToArray();

            return result;
        }

        public async Task<SeoInfo[]> GetBestMatchingSeoInfos(string slug, Store store, string currentCulture)
        {
            var result = (await _coreModuleApi.GetSeoInfoBySlugAsync(slug)).GetBestMatchingSeoInfos(store, currentCulture, slug).Select(x => x.ToSeoInfo()).ToArray();

            return result;
        }

        public ContentItem GetContentItem(string slug, WorkContext context)
        {
            ContentItem result = null;
            var pageUrl = slug == "__index__home__page__" ? "/" : $"/{slug}";
            try
            {
                var pages = context.Pages.Where(p =>
                    string.Equals(p.Url, pageUrl, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(p.Url, slug, StringComparison.OrdinalIgnoreCase)
                );

                var page = pages.FirstOrDefault(x => x.Language.CultureName.EqualsInvariant(context.CurrentLanguage.CultureName))
                           ?? pages.FirstOrDefault(x => x.Language.IsInvariant)
                           ?? pages.FirstOrDefault(x => x.AliasesUrls.Contains(pageUrl, StringComparer.OrdinalIgnoreCase));
                result = page;

            }
            catch
            {
                //do nothing
            }

            return result;
        }
    }
}
