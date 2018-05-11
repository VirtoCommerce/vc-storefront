using System;
using Microsoft.WindowsAzure.Storage.Blob;

namespace VirtoCommerce.Storefront.Domain
{
    public class AzureBlobContentOptions
    {
        public string Container { get; set; }
        public string ConnectionString { get; set; }
        public bool PollForChanges { get; set; } = false;
        public TimeSpan ChangesPoolingInterval { get; set; } = TimeSpan.FromSeconds(15);
        public BlobRequestOptions BlobRequestOptions { get; set; } = new BlobRequestOptions();
    }
}
