using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseStorefrontRoles(this IApplicationBuilder appBuilder)
        {
            var platformSecurityApi = appBuilder.ApplicationServices.GetRequiredService<ISecurity>();

            foreach (var role in SecurityConstants.Roles.AllRoles)
            {
                platformSecurityApi.UpdateRole(role.ToRoleDto());
            }
            return appBuilder;
        }
    }
}
