using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Routing
{
    public interface ISlugRouteService
    {
        SlugRouteResponse HandleSlugRequest(string slugPath, WorkContext workContext);
    }
}
