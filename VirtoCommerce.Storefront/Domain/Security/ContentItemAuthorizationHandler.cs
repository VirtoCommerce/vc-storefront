using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain.Security
{
    public class ContentItemAuthorizeRequirement : IAuthorizationRequirement { }

    public class ContentItemAuthorizationHandler : AuthorizationHandler<ContentItemAuthorizeRequirement, ContentItem>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ContentItemAuthorizeRequirement requirement, ContentItem resource)
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
