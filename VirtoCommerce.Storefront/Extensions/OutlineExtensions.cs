using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Tools;
using catalogDto = VirtoCommerce.Storefront.AutoRestClients.CatalogModuleApi.Models;
using toolsDto = VirtoCommerce.Tools.Models;

namespace VirtoCommerce.Storefront.Common
{
    public static class OutlineExtensions
    {
        /// <summary>
        /// Returns best matching outline path for the given catalog: CategoryId/CategoryId2.
        /// </summary>
        /// <param name="outlines"></param>
        /// <param name="catalogId"></param>
        /// <returns></returns>
        public static string GetOutlinePath(this IEnumerable<catalogDto.Outline> outlines, string catalogId)
        {
            return outlines?.Select(o => o.JsonConvert<toolsDto.Outline>()).GetOutlinePath(catalogId);
        }

        /// <summary>
        /// Returns product's category outline.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static string GetCategoryOutline(this Product product)
        {
            var result = string.Empty;

            if (product != null && !string.IsNullOrEmpty(product.Outline))
            {
                var i = product.Outline.LastIndexOf('/');
                if (i >= 0)
                {
                    result = product.Outline.Substring(0, i);
                }
            }

            return result;
        }
    }
}
