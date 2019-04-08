using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace VirtoCommerce.Storefront.Filters
{
    public class DenyAnonymousForStoreAuthorizeFilter : IAsyncAuthorizationFilter
    {
        public AuthorizationPolicy Policy { get; }

        public DenyAnonymousForStoreAuthorizeFilter(AuthorizationPolicy policy)
        {
            Policy = policy ?? throw new ArgumentNullException(nameof(policy));
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Allow Anonymous skips all authorization
            if (context.Filters.Any(x => x is IAllowAnonymousFilter))
            {
                return;
            }

            var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateResult = await policyEvaluator.AuthenticateAsync(Policy, context.HttpContext);
            var authorizeResult = await policyEvaluator.AuthorizeAsync(Policy, authenticateResult, context.HttpContext, context);


            // For all the results except Succeeded we need to return 
            if (!authorizeResult.Succeeded)
            {
                // Here we need only ChallengeResult to redirect to login instead of ForbiddenResult that standard AuthorizeFilter returns in that case (authenticated and non authorized user)
                // https://github.com/aspnet/AspNetCore/blob/v2.2.3/src/Mvc/Mvc.Core/src/Authorization/AuthorizeFilter.cs#L210
                // https://github.com/aspnet/AspNetCore/blob/v2.2.3/src/Security/Authorization/Policy/src/PolicyEvaluator.cs#L91
                context.Result = new ChallengeResult(Policy.AuthenticationSchemes.ToArray());
            }
        }
    }
}
