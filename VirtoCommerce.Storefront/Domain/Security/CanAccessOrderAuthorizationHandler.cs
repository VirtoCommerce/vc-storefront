using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain.Security
{
    /// <summary>
    /// Auth policies requirement for checking if can user access to the given order 
    /// </summary>
    public class CanAccessOrderAuthorizationRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "CanAccessOrder";
    }

    public class CanAccessOrderAuthorizationHandler : AuthorizationHandler<CanAccessOrderAuthorizationRequirement, CustomerOrder>
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public CanAccessOrderAuthorizationHandler(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessOrderAuthorizationRequirement requirement, CustomerOrder resource)
        {
            if (resource != null && resource.CustomerId == _workContextAccessor.WorkContext.CurrentUser?.Id)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
