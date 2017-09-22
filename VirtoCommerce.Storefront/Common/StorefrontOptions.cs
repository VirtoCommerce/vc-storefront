using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;

namespace VirtoCommerce.Storefront.Common
{
    public class StorefrontOptions
    {
        public string DefaultStore { get; set; }
        public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
    }
}
