using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Common
{
    public class PlatformEndpointOptions
    {
        public Uri Url { get; set; }
        public string AppId { get; set; }
        public string SecretKey { get; set; }
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
