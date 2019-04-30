using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Domain.Security;

namespace VirtoCommerce.Storefront.Filters
{
    /// <summary>
    /// Authorization filter that redirects all unauthorized users to Login page (deafult AuthorizeFilter could show AccessDenied for non-authorized authenticated users)
    /// </summary>
    public class ForceLoginAuthorizationHandler : IAsyncAuthorizationFilter
    {
        public readonly IAuthorizationPolicyProvider _policyProvider;

        public ForceLoginAuthorizationHandler(IAuthorizationPolicyProvider policyProvider)
        {
            _policyProvider = policyProvider;
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

            var policy = await _policyProvider.GetPolicyAsync(AnonymousUserForStoreAuthorizationRequirement.PolicyName);
            var policyEvaluator = context.HttpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, context.HttpContext);
            var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, context.HttpContext, context);

            // For all the results except Succeeded we need to return <see cref="ChallengeResult"/>
            if (!authorizeResult.Succeeded)
            {
                // Here we need only ChallengeResult to redirect to Login instead of ForbiddenResult that standard AuthorizeFilter returns in that case (authenticated and non authorized user)
                // https://github.com/aspnet/AspNetCore/blob/v2.2.3/src/Mvc/Mvc.Core/src/Authorization/AuthorizeFilter.cs#L210
                // https://github.com/aspnet/AspNetCore/blob/v2.2.3/src/Security/Authorization/Policy/src/PolicyEvaluator.cs#L91
                context.Result = new ChallengeResult(policy.AuthenticationSchemes.ToArray());
            }
        }
    }
}
