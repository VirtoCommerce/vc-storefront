using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.AutoRestClients.CoreModuleApi;
using VirtoCommerce.Storefront.Model;

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
    }
}
