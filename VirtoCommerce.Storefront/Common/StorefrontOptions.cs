using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Common
{
    public class StorefrontOptions
    {
        public StorefrontOptions()
        {
            Api = new PlatformApiOptions();
            Settings = new Dictionary<string, object>();
        }
        public PlatformApiOptions Api { get; set; }
        public string DefaultStore { get; set; }
        public Dictionary<string, object> Settings { get; set; }
    }

    public class PlatformApiOptions
    {
        public PlatformApiOptions()
        {
            RequestTimeout = TimeSpan.FromSeconds(30);
        }
        public string Url { get; set; }
        public string AppId { get; set; }
        public string SecretKey { get; set; }
        public TimeSpan RequestTimeout { get; set; }
    }

}
