using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VirtoCommerce.Storefront.Domain;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class BlobChangesWatcher : IBlobChangesWatcher
    {
        private readonly AzureBlobContentOptions _options;
        private readonly CloudBlobContainer _container;

        public BlobChangesWatcher(IOptions<AzureBlobContentOptions> options)
        {
            _options = options.Value;

            if (CloudStorageAccount.TryParse(_options.ConnectionString, out CloudStorageAccount cloudStorageAccount))
            {
                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                _container = cloudBlobClient.GetContainerReference(_options.Container);
            }
        }

        public IChangeToken CreateBlobChangeToken(string path)
        {
            if (!_options.PollForChanges || _container == null)
            {
                return new CancellationChangeToken(new CancellationToken());
            }

            return new BlobChangeToken(path, _container, _options);
        }
    }
}
