using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model
{
    public interface ISpaRoutesService
    {
        Task<bool> IsSpaRoute(string route);
    }
}
