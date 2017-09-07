using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.Middleware
{
    public class StorefrontUrlNormalizeRule : IRule
    {
        #region IRule Members
        public void ApplyRule(RewriteContext context)
        {
            var workContext = context.HttpContext.RequestServices.GetService<IWorkContextAccessor>().WorkContext;

            var currentStoreId = workContext.CurrentStore != null ? workContext.CurrentStore.Id : "-";
            var currentCultureName = workContext.CurrentLanguage != null ? workContext.CurrentLanguage.CultureName : "en-US";

            var normalizedPath = new PathString();
            //add store to path
            normalizedPath = normalizedPath.Add(new PathString("/" + currentStoreId));
            //add language to path
            normalizedPath = normalizedPath.Add(new PathString("/" + currentCultureName));

            //add remaining path part without store and language
            var requestPath = context.HttpContext.Request.Path;
            requestPath = Regex.Replace(requestPath, @"/\b" + currentStoreId + @"\b/?", "/", RegexOptions.IgnoreCase);
            requestPath = Regex.Replace(requestPath, @"/\b" + currentCultureName + @"\b/?", "/", RegexOptions.IgnoreCase);
            normalizedPath = normalizedPath.Add(new PathString(requestPath));

            context.HttpContext.Request.Path = normalizedPath;
            context.Result = RuleResult.ContinueRules;
        }
        #endregion

    }
}
