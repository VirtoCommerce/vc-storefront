using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Security;

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
            var user = _workContextAccessor.WorkContext.CurrentUser;

            var result = user.IsAdministrator;
            if (!result)
            {
                result = user.Permissions.Contains(SecurityConstants.Permissions.CanViewOrders);
            }
            if (!result)
            {
                result = resource != null && resource.CustomerId == user.Id;
            }
            if (result)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
