namespace VirtoCommerce.Storefront.Domain.Security
{
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// The order status change authorization requirement.
    /// </summary>
    public class OrderStatusChangeAuthorizationRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "CanChangeOrderStatus";
    }
}
