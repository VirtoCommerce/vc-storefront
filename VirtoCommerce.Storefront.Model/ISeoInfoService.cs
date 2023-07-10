using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Model
{
    public interface ISeoInfoService
    {
        Task<SeoInfo[]> GetSeoInfosBySlug(string slug);

        Task<SeoInfo[]> GetBestMatchingSeoInfos(string slug, Store store, string currentCulture);

        ContentItem GetContentItem(string slug, WorkContext context);
    }
}
