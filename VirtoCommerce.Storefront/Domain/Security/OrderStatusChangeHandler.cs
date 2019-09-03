namespace VirtoCommerce.Storefront.Domain.Security
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;

    using VirtoCommerce.Storefront.AutoRestClients.OrdersModuleApi.Models;
    using VirtoCommerce.Storefront.Model;
    using VirtoCommerce.Storefront.Model.Security;

    public class OrderStatusChangeHandler : AuthorizationHandler<OrderStatusChangeAuthorizationRequirement, CustomerOrder>
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public OrderStatusChangeHandler(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrderStatusChangeAuthorizationRequirement requirement, CustomerOrder resource)
        {
            var currentUser = _workContextAccessor.WorkContext.CurrentUser;
            var isContainsPermissionToChangeOrderStatus = currentUser?.Permissions.Any(permission => permission == SecurityConstants.Permissions.CanChangeOrderStatus) ?? false;
            if (resource != null && isContainsPermissionToChangeOrderStatus)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
