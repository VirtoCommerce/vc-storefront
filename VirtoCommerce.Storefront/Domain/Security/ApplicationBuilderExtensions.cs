using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi;
using dto = VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi.Models;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseStorefrontRoles(this IApplicationBuilder appBuilder)
        {
            var platformSecurityApi = appBuilder.ApplicationServices.GetRequiredService<ISecurity>();
            var organizationMaintainerRole =
                new dto.Role {
                    Id = SecurityConstants.Roles.OrganizationMaintainer,
                    Name = SecurityConstants.Roles.OrganizationMaintainer,
                    Permissions = new[] {
                                         new dto.Permission { Id = SecurityConstants.Permissions.CanEditOrganization, Name = "Can edit organization resources" }
                                        }
                };
            var organizationEmployeeRole = new dto.Role { Id = SecurityConstants.Roles.OrganizationEmployee, Name = "Organization employee" };

            platformSecurityApi.UpdateRole(organizationMaintainerRole);
            platformSecurityApi.UpdateRole(organizationEmployeeRole);

            return appBuilder;
        }
    }
}
