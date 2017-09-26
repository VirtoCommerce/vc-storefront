using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Common
{
    public class AzureBlobContentOptions
    {
        public string Container { get; set; }
        public string Directory { get; set; }
        public string ConnectionString { get; set; }
        public bool PollForChanges { get; set; } = false;
        public int PollingChangesInterval { get; set; } = 5000;
        public BlobRequestOptions BlobRequestOptions { get; set; } = new BlobRequestOptions();
    }
}
