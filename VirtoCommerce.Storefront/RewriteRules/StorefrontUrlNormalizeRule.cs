using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Middleware
{
    public class StorefrontUrlNormalizeRule : IRule
    {
        #region IRule Members
        public void ApplyRule(RewriteContext context)
        {
            var workContext = context.HttpContext.RequestServices.GetService<IWorkContextAccessor>().WorkContext;
            if (workContext != null)
            {
                context.HttpContext.Request.Path = context.HttpContext.Request.Path.AddStoreAndLangSegment(workContext.CurrentStore, workContext.CurrentLanguage);
            }
            context.Result = RuleResult.ContinueRules;
        }
        #endregion

    }
}
