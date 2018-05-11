using System;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class PlatformEndpointOptions
    {
        public Uri Url { get; set; }
        public string AppId { get; set; }
        public string SecretKey { get; set; }
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
