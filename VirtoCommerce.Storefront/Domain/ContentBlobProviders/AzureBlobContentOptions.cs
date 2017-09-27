using Microsoft.WindowsAzure.Storage.Blob;

namespace VirtoCommerce.Storefront.Domain
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
