using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class OnlyRegisteredUserAuthorizationRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "OnlyRegisteredUser";
    }
    /// <summary>
    /// We need this policy as alternative for [Authorize] attribute, because in the storefront all users are registered in terms of ASP.NET authorization.
    /// </summary>
    public class OnlyRegisteredUserAuthorizationHandler : AuthorizationHandler<OnlyRegisteredUserAuthorizationRequirement>
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        public OnlyRegisteredUserAuthorizationHandler(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OnlyRegisteredUserAuthorizationRequirement requirement)
        {
            var workContext = _workContextAccessor.WorkContext;
            if (workContext.CurrentUser.IsRegisteredUser)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
