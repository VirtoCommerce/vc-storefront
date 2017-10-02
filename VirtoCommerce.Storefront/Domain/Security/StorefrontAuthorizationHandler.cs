using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class StorefrontAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly IStorefrontSecurityService _authService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public StorefrontAuthorizationHandler(IStorefrontSecurityService authService, IWorkContextAccessor workContextAccessor)
        {
            _authService = authService;
            _workContextAccessor = workContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            // Validate the requirement against the resource and identity.
            if(requirement == AuthorizationOperations.CanImpersonate)
            {
                var workContext = _workContextAccessor.WorkContext;
                var result = await _authService.CanLoginOnBehalfAsync(workContext.CurrentStore.Id, workContext.CurrentUser.Id);
                if(result)
                {
                    context.Succeed(requirement);
                }           
            }            
        }
    }
}
