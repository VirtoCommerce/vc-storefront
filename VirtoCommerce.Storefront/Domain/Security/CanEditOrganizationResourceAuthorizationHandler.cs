using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Customer;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CanEditOrganizationResourceAuthorizeRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "CanEditOrganizationResource";
    }

    public class CanEditOrganizationResourceAuthorizationHandler : AuthorizationHandler<CanEditOrganizationResourceAuthorizeRequirement, Organization>
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        public CanEditOrganizationResourceAuthorizationHandler(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditOrganizationResourceAuthorizeRequirement requirement, Organization resource)
        {

            var workContext = _workContextAccessor.WorkContext;
            //Allow to do all things with self 

            var currentUserOrgIds = workContext.CurrentUser?.Contact?.OrganizationsIds;
            var result = currentUserOrgIds != null && resource != null && currentUserOrgIds.Contains(resource.Id);

            if (result)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
