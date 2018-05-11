using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Routing
{
    public interface ISlugRouteService
    {
        Task<SlugRouteResponse> HandleSlugRequestAsync(string slugPath, WorkContext workContext);
    }
}
