using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model
{
    public interface ISeoInfoService
    {
        Task<SeoInfo[]> GetSeoInfosBySlug(string slug);
    }
}
