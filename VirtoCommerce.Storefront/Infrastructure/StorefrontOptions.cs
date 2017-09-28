using System;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class StorefrontOptions
    {
        public string DefaultStore { get; set; }
        public TimeSpan ChangesPoolingInterval { get; set; } = TimeSpan.FromMinutes(1);
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
    }
}
