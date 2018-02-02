using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VirtoCommerce.Storefront.Domain;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Infrastructure
{
    /// <summary>
    /// Based on PollingFileChangeToken 
    /// </summary>
    public class BlobChangeToken : IChangeToken
    {
        private static ConcurrentDictionary<string, DateTime> _previousChangeTimeUtcTokenLookup = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        public string BlobName { get; set; }
        private bool _hasChanged;
        private readonly CloudBlobContainer _container;
        private readonly AzureBlobContentOptions _options;
        private DateTime _lastModifiedUtc;
        private DateTime _prevModifiedUtc;
        private static DateTime _lastCheckedTimeUtcStatic;
        private static object _lock = new object();

        public BlobChangeToken(string blobName, CloudBlobContainer container, AzureBlobContentOptions options)
        {
            BlobName = blobName;

            _container = container;
            _options = options;

            _lastModifiedUtc = _prevModifiedUtc = DateTime.UtcNow;
        }

        public bool HasChanged
        {
            get
            {
                //get last modified dt
                _lastModifiedUtc = _previousChangeTimeUtcTokenLookup.GetOrAdd(BlobName, _lastModifiedUtc);

                var hasChanged = _lastModifiedUtc > _prevModifiedUtc;
                if (hasChanged)
                {
                    _prevModifiedUtc = _lastModifiedUtc;
                    _hasChanged = true;
                }

                //check pooling interval
                var currentTime = DateTime.UtcNow;
                if (currentTime - _lastCheckedTimeUtcStatic < _options.ChangesPoolingInterval)
                {
                    return _hasChanged;
                }

                bool lockTaken = Monitor.TryEnter(_lock);
                try
                {
                    if (lockTaken)
                    {
                        Task.Run(() => EvaluateBlobsModifiedDate());
                        _lastCheckedTimeUtcStatic = currentTime;
                    }
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(_lock);
                }

                return _hasChanged;
            }
        }

        private void EvaluateBlobsModifiedDate(CancellationToken cancellationToken = default(CancellationToken))
        {
            var files = ListBlobs().GetAwaiter().GetResult();
            foreach (var file in files)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var lastModifiedUtc = file.Properties.LastModified.HasValue ? file.Properties.LastModified.Value.UtcDateTime : DateTime.MinValue;

                if (!_previousChangeTimeUtcTokenLookup.TryGetValue(file.Name, out DateTime dt))
                {
                    _previousChangeTimeUtcTokenLookup.GetOrAdd(file.Name, lastModifiedUtc);
                }
                else
                {
                    _previousChangeTimeUtcTokenLookup[file.Name] = lastModifiedUtc;
                }
            }
        }

        private async Task<IEnumerable<CloudBlob>> ListBlobs()
        {
            var context = new OperationContext();
            var blobItems = new List<IListBlobItem>();
            BlobContinuationToken token = null;
            var operationContext = new OperationContext();
            do
            {
                var resultSegment = await _container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, token, _options.BlobRequestOptions, operationContext);
                token = resultSegment.ContinuationToken;
                blobItems.AddRange(resultSegment.Results);
            } while (token != null);

            var result = blobItems.OfType<CloudBlob>().ToList();
            return result;
        }

        /// <summary>
        /// Don't know what to do with this one, so false
        /// </summary>
        public bool ActiveChangeCallbacks => false;

        /// <summary>
        /// Don't know  what to do with this either
        /// </summary>
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => EmptyDisposable.Instance;
    }
}
