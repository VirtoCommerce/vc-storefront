using System.Text.Encodings.Web;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
            WorkContext = new WorkContext();

            WorkContext.RequestUrl = HttpContext.Request.GetUri();
            var htmlEncoder = httpContext.RequestServices.GetRequiredService<HtmlEncoder>();
            var qs = WorkContext.QueryString = HttpContext.Request.Query.ToNameValueCollection(htmlEncoder);
            WorkContext.PageNumber = qs["page"].ToNullableInt();

            var qsPageSize = qs["count"].ToNullableInt() ?? qs["page_size"].ToNullableInt();
            if (qsPageSize.HasValue && qsPageSize.Value > options.PageSizeMaxValue)
            {
                WorkContext.PageSize = options.PageSizeMaxValue;
            }
        }

        public HttpContext HttpContext { get; }
        public WorkContext WorkContext { get; }
    }
}
