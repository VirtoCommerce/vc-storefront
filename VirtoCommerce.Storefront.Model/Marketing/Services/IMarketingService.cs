using System;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Model.Services
{
    public interface IMarketingService
    {
        [Obsolete("Use IDynamicContentEvaluator instead")]
        Task<string> GetDynamicContentHtmlAsync(string storeId, string placeholderName);
    }
}
