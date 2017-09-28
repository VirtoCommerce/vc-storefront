using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Customer.Services;

namespace VirtoCommerce.Storefront.Authorization
{
    public class StorefrontAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly ICustomerService _customerService;
        private readonly IWorkContextAccessor _workContextAccessor;
        public StorefrontAuthorizationHandler(ICustomerService customerService, IWorkContextAccessor workContextAccessor)
        {
            _customerService = customerService;
            _workContextAccessor = workContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            // Validate the requirement against the resource and identity.
            if(requirement == AuthorizationOperations.CanImpersonate)
            {
                var workContext = _workContextAccessor.WorkContext;
                var result = await _customerService.CanLoginOnBehalfAsync(workContext.CurrentStore.Id, workContext.CurrentCustomer.Id);
                if(result)
                {
                    context.Succeed(requirement);
                }           
            }            
        }
    }
}
