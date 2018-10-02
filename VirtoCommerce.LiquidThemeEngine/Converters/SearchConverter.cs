using PagedList.Core;
using System.Linq;
using VirtoCommerce.LiquidThemeEngine.Objects;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using dotLiquid = DotLiquid;
using storefrontModel = VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.LiquidThemeEngine.Converters
{
    public static class SearchStaticConverter
    {
        public static Search ToShopifyModel(this IMutablePagedList<storefrontModel.Product> products, WorkContext workContext)
        {
            var converter = new ShopifyModelConverter();
            return converter.ToLiquidSearch(products, workContext);
        }
    }

    public partial class ShopifyModelConverter
    {
        public virtual Search ToLiquidSearch(IMutablePagedList<storefrontModel.Product> products, WorkContext workContext)
        {
            var result = new Search();

            result.Performed = true;
            result.Terms = workContext.CurrentProductSearchCriteria.Keyword;

            result.Results = new MutablePagedList<dotLiquid.Drop>((pageNumber, pageSize, sortInfos, @params) =>
            {
                products.Slice(pageNumber, pageSize, sortInfos, @params);
                return new StaticPagedList<dotLiquid.Drop>(products.Select(x => x.ToShopifyModel()), products);
            }, 1, products.PageSize);

            if (workContext.Aggregations != null)
            {
                result.AllTags = new TagCollection(new MutablePagedList<Tag>((pageNumber, pageSize, sortInfos, @params) =>
                {
                    workContext.Aggregations.Slice(pageNumber, pageSize, sortInfos, @params);
                    var tags = workContext.Aggregations
                        .Where(a => a.Items != null)
                        .SelectMany(a => a.Items.Select(item => item.ToShopifyModel(a)));
                    return new StaticPagedList<Tag>(tags, workContext.Aggregations);

                }, workContext.Aggregations.PageNumber, workContext.Aggregations.PageSize));
            }

            return result;
        }
    }
}
