using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CanReadContentItemAuthorizeRequirement : IAuthorizationRequirement
    {
        public const string PolicyName = "CanReadContentItem";
    }

    public class CanReadContentItemAuthorizationHandler : AuthorizationHandler<CanReadContentItemAuthorizeRequirement, ContentItem>
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        public CanReadContentItemAuthorizationHandler(IWorkContextAccessor workContextAccessor)
        {
            _workContextAccessor = workContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadContentItemAuthorizeRequirement requirement, ContentItem resource)
        {
            var workContext = _workContextAccessor.WorkContext;
            if (resource.Authorize)
            {
                if (workContext.CurrentUser?.IsRegisteredUser == true)
                {
                    context.Succeed(requirement);
                }
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
