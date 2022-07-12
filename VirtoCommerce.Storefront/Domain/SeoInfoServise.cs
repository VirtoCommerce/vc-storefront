using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public class SeoInfoServise : ISeoInfoService
    {
        private readonly ICommerce _coreModuleApi;
        public SeoInfoServise(ICommerce coreModuleApi)
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
    }
}
