using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model
{
    public interface ISpaRouteService
    {
        Task<bool> IsSpaRoute(string route);
    }
}
