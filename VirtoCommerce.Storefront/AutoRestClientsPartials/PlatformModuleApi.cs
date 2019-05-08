using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Infrastructure;

namespace VirtoCommerce.Storefront.AutoRestClients.PlatformModuleApi
{
    /// <summary>
    /// partial extention of AutoRest generated ServiceClient reailization for adding of ctr with HttpClient argument
    /// </summary>
    public partial class VirtoCommercePlatformRESTAPIdocumentation
    {
        public VirtoCommercePlatformRESTAPIdocumentation(HttpClient httpClient, IOptions<PlatformEndpointOptions> options)
            : base(httpClient)
        {
            var platformEndpointOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
            BaseUri = platformEndpointOptions.Url;
        }
    }
}
