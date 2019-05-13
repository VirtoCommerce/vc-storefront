//using System;
//using System.Net.Http;
//using Microsoft.Extensions.Options;
//using VirtoCommerce.Storefront.Infrastructure;
//using VirtoCommerce.Storefront.Infrastructure.Autorest;

//namespace VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi
//{
//    /// <summary>
//    /// partial extention of AutoRest generated ServiceClient reailization to wrap ctr with HttpClient argument and get ctr with suitable view fro AddHttpClient usage
//    /// </summary>
//    public partial class VirtoCommerceSubscriptionRESTAPIdocumentation
//    {
//        public VirtoCommerceSubscriptionRESTAPIdocumentation(HttpClient httpClient, IOptions<PlatformEndpointOptions> options)
//            : this(new EmptyServiceClientCredentials(), httpClient, false)
//        {
//            var platformEndpointOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
//            BaseUri = platformEndpointOptions.Url;
//        }
//    }
//}

//using System.Net.Http;
using System;
using System.Net.Http;
using Microsoft.Extensions.Options;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Infrastructure.Autorest;

namespace VirtoCommerce.Storefront.AutoRestClients.SubscriptionModuleApi
{
    public class VirtoCommerceSubscriptionRESTAPIdocumentationExtended : VirtoCommerceSubscriptionRESTAPIdocumentation
    {
        public VirtoCommerceSubscriptionRESTAPIdocumentationExtended(HttpClient httpClient, IOptions<PlatformEndpointOptions> options)
            : base(new EmptyServiceClientCredentials(), httpClient, false)
        {
            var platformEndpointOptions = options.Value ?? throw new ArgumentNullException(nameof(options));
            BaseUri = platformEndpointOptions.Url;
        }
    }
}
