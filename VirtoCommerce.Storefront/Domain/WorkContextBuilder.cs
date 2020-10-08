using System.Linq;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class WorkContextBuilder : IWorkContextBuilder
    {
        public WorkContextBuilder(HttpContext httpContext, StorefrontOptions options)
        {
            HttpContext = httpContext;

            var qs = HttpContext.Request.Query.ToDictionary(x=> x.Key, x=> x.Value.ToString()).WithDefaultValue(null);

            WorkContext = new WorkContext
            {
                RequestUrl = HttpContext.Request.GetUri(),
                QueryString = qs,
                PageNumber = qs["page"].ToNullableInt() ?? 1,
            };

            var pageSize = qs["count"].ToNullableInt() ?? qs["page_size"].ToNullableInt();
            if (pageSize != null && pageSize.Value > options.PageSizeMaxValue)
            {
                pageSize = options.PageSizeMaxValue;
            }
            WorkContext.PageSize = pageSize ?? 20;
            //To interpret as true the value of preview_mode from the query string according to its actual presence, since another value of this parameter can be passed.
            WorkContext.IsPreviewMode = !string.IsNullOrEmpty(WorkContext.QueryString["preview_mode"]);

        }

        public HttpContext HttpContext { get; }
        public WorkContext WorkContext { get; }
    }
}
