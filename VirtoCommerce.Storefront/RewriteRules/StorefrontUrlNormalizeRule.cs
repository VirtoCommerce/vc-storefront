using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Extensions;

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
