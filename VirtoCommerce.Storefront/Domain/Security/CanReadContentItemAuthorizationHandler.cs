using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class CanReadContentItemAuthorizeRequirement : IAuthorizationRequirement {
        public const string PolicyName = "CanReadContentItem";
    }

    public class CanReadContentItemAuthorizationHandler : AuthorizationHandler<CanReadContentItemAuthorizeRequirement, ContentItem>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanReadContentItemAuthorizeRequirement requirement, ContentItem resource)
        {
            if (resource.Authorize)
            {
                if(context.User.Identity.IsAuthenticated)
                    context.Succeed(requirement);
            }
            else 
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
