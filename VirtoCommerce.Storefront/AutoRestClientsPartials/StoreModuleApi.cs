using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Infrastructure;

namespace VirtoCommerce.Storefront.AutoRestClients.StoreModuleApi
{
    /// <summary>
    /// partial extention of AutoRest generated ServiceClient<T> reailization for adding of ctr with HttpClient argument
    /// </summary>
    public partial class VirtoCommerceStoreRESTAPIdocumentation
    {
        public VirtoCommerceStoreRESTAPIdocumentation(HttpClient httpClient, IOptions<PlatformEndpointOptions> options)
            : base(httpClient)
        {
            var platformEndpointOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
            BaseUri = platformEndpointOptions.Url;
        }
    }
}
