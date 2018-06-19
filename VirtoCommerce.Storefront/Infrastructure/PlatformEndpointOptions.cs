using System;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class PlatformEndpointOptions
    {
        public ApiAuthMode AuthMode { get; set; } = ApiAuthMode.BarrierToken;
        public Uri Url { get; set; }
        /// <summary>
        /// Credentials used for ApiAuthMode.BarrierToken mode
        /// </summary>
        public string AppId { get; set; }
        public string SecretKey { get; set; }

        /// <summary>
        /// Credentials used for ApiAuthMode.OAuthPassword mode
        /// </summary>
        public string UserName { get; set; }
        public string Password { get; set; }

        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
