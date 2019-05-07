using System.Net.Http;
using Microsoft.Extensions.Options;

namespace VirtoCommerce.Storefront.Infrastructure.Autorest
{
    public abstract class AbstractAuthHandler : DelegatingHandler
    {
        protected readonly PlatformEndpointOptions _options;
        protected readonly IHttpClientFactory _clientFactory;

        protected AbstractAuthHandler(IOptions<PlatformEndpointOptions> options, IHttpClientFactory clientFactory)
        {
            _options = options.Value;
            _clientFactory = clientFactory;
        }


    }
}
