using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
        {
            if (context.User.IsInRole(SecurityConstants.Roles.Administrator))
            {
                context.Succeed(requirement);
            }
            if (context.User.HasClaim(SecurityConstants.Claims.PermissionClaimType, requirement.Permission))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }


    }
}
