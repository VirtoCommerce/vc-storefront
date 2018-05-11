using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Tools;
using catalogDto = VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi.Models;
using coreDto = VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi.Models;
using toolsDto = VirtoCommerce.Tools.Models;

namespace VirtoCommerce.Storefront.Common
{
    public static class SeoExtensions
    {
        /// <summary>
        /// Returns SEO path if all outline items of the first outline have SEO keywords, otherwise returns default value.
        /// Path: GrandParentCategory/ParentCategory/ProductCategory/Product
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="store"></param>
        /// <param name="language"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetSeoPath(this IEnumerable<catalogDto.Outline> outlines, Store store, Language language, string defaultValue)
        {
            return outlines
                ?.Select(o => o.JsonConvert<toolsDto.Outline>())
                .GetSeoPath(store.ToToolsStore(), language.CultureName, defaultValue);
        }

        /// <summary>
        /// Returns SEO records with highest score
        /// http://docs.virtocommerce.com/display/vc2devguide/SEO
        /// </summary>
        /// <param name="seoRecords"></param>
        /// <param name="store"></param>
        /// <param name="language"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static IList<coreDto.SeoInfo> GetBestMatchingSeoInfos(this IEnumerable<coreDto.SeoInfo> seoRecords, Store store, Language language, string slug = null)
        {
            return seoRecords
                ?.Select(s => s.JsonConvert<toolsDto.SeoInfo>())
                .GetBestMatchingSeoInfos(store.Id, store.DefaultLanguage.CultureName, language.CultureName, slug)
                .Select(s => s.JsonConvert<coreDto.SeoInfo>())
                .ToList();
        }
    }
}
