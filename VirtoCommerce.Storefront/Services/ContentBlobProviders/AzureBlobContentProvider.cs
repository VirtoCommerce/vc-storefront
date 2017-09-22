using CacheManager.Core;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Services
{
    public class AzureBlobContentProvider : IContentBlobProvider, IDisposable
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudBlobContainer _container;
        private readonly CloudBlobDirectory _directory;
        private readonly ICacheManager<object> _cacheManager;
        private readonly CancellationTokenSource _cancelSource;
        private readonly AzureBlobContentOptions _options;
        public AzureBlobContentProvider(IOptions<AzureBlobContentOptions> options, ICacheManager<object> cacheManager)
        {
            _options = options.Value;
            _cacheManager = cacheManager;       

            if (!CloudStorageAccount.TryParse(_options.ConnectionString, out _cloudStorageAccount))
            {
                throw new StorefrontException("Failed to get valid connection string");
            }
            _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
            _container = _cloudBlobClient.GetContainerReference(_options.Container);
            if (!string.IsNullOrEmpty(_options.Directory))
            {
                _directory = _container.GetDirectoryReference(_options.Directory);
            }
            _cancelSource = new CancellationTokenSource();

            if (_options.TrackChanges)
            {
                Task.Run(() => MonitorFileSystemChanges(_cancelSource.Token), _cancelSource.Token);
            }
        }

        private void MonitorFileSystemChanges(CancellationToken cancellationToken)
        {
            var intetval = _options.TrackChangesInterval;

            var latestModifiedDate = DateTimeOffset.UtcNow;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var maxModifiedDate = DateTimeOffset.MinValue;

                    foreach (var file in EnumBlobFiles())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            break;

                        if (file.Properties.LastModified.HasValue)
                        {
                            if (maxModifiedDate < file.Properties.LastModified)
                                maxModifiedDate = (DateTimeOffset)file.Properties.LastModified;

                            if (file.Properties.LastModified > latestModifiedDate)
                            {
                                RaiseChangedEvent(new FileSystemEventArgs(WatcherChangeTypes.Changed, Path.GetDirectoryName(file.Name), Path.GetFileName(file.Name)));
                            }
                        }
                    }

                    latestModifiedDate = maxModifiedDate;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }

                Thread.Sleep(intetval);
            }
        }

        private IEnumerable<CloudBlob> EnumBlobFiles()
        {
            return Task.Factory.StartNew(() => EnumBlobFilesAsync(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
        private async Task<IEnumerable<CloudBlob>> EnumBlobFilesAsync()
        {
            var result = new List<CloudBlob>();
            var token = new BlobContinuationToken();
            var context = new OperationContext();
            if (_directory != null)
            {
                do
                {
                    var resultSegment = await _directory.ListBlobsSegmentedAsync(true, BlobListingDetails.Metadata, null, token, _options.BlobRequestOptions, context);
                    token = resultSegment.ContinuationToken;
                    result.AddRange(resultSegment.Results.OfType<CloudBlob>());
                } while (token != null);
            }
            else
            {
                do
                {
                    var resultSegment = await _container.ListBlobsSegmentedAsync(null, true, BlobListingDetails.Metadata, null, token, _options.BlobRequestOptions, context);
                    token = resultSegment.ContinuationToken;
                    result.AddRange(resultSegment.Results.OfType<CloudBlob>());
                } while (token != null);
            }
            return result;
        }

        #region IContentBlobProvider Members
        public event FileSystemEventHandler Changed;
        public event RenamedEventHandler Renamed;


    
        /// <summary>
        /// Open blob for read 
        /// </summary>
        /// <param name="path">blob relative path /folder/blob.md</param>
        /// <returns></returns>
        public virtual Stream OpenRead(string path)
        {
            return Task.Factory.StartNew(() => OpenReadAsync(path), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async virtual Task<Stream> OpenReadAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            path = NormalizePath(path);
            if (_directory != null)
            {
                return await _directory.GetBlockBlobReference(path).OpenReadAsync();
            }

            return await _container.GetBlobReference(path).OpenReadAsync();
        }

        /// <summary>
        /// Open blob for write by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>blob stream</returns>
        public virtual Stream OpenWrite(string path)
        {
            return Task.Factory.StartNew(() => OpenWriteAsync(path), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async virtual Task<Stream> OpenWriteAsync(string path)
        {
            //Container name
            path = NormalizePath(path);
            var blob = _container.GetBlockBlobReference(path);
            blob.Properties.ContentType = MimeTypes.GetMimeType(Path.GetFileName(path));
            return await blob.OpenWriteAsync();
        }

        /// <summary>
        /// Check that blob or folder with passed path exist
        /// </summary>
        /// <param name="path">path /folder/blob.md</param>
        /// <returns></returns>
        public virtual bool PathExists(string path)
        {
            return Task.Factory.StartNew(() => PathExistsAsync(path), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }

        public async virtual Task<bool> PathExistsAsync(string path)
        {
            path = NormalizePath(path);

            var result = await _cacheManager.GetAsync("AzureBlobContentProvider.PathExists:" + path.GetHashCode(), "ContentRegion", async () =>
            {
                // If requested path is a directory we should always return true because Azure blob storage does not support checking if directories exist
                var retVal = string.IsNullOrEmpty(Path.GetExtension(path));
                if (!retVal)
                {
                    var url = GetAbsoluteUrl(path);
                    try
                    {
                        retVal = await (await _cloudBlobClient.GetBlobReferenceFromServerAsync(new Uri(url))).ExistsAsync();
                    }
                    catch (Exception)
                    {
                        //Azure blob storage client does not provide method to check blob url exist without throwing exception
                    }
                }

                return (object)retVal;
            });

            return (bool)result;
        }


        /// <summary>
        /// Search blob content in specified folder
        /// </summary>
        /// <param name="path">folder path in which the search will be processed</param>
        /// <param name="searchPattern">search blob name pattern can be used mask (*, ? symbols)</param>
        /// <param name="recursive"> recursive search</param>
        /// <returns>Returns relative path for all found blobs  example: /folder/blob.md </returns>
        public virtual IEnumerable<string> Search(string path, string searchPattern, bool recursive)
        {
            return Task.Factory.StartNew(() => SearchAsync(path, searchPattern, recursive), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default).Unwrap().GetAwaiter().GetResult();
        }
       
        public virtual async Task<IEnumerable<string>> SearchAsync(string path, string searchPattern, bool recursive)
        {
            var retVal = new List<string>();
            path = NormalizePath(path);
            //Search pattern may contains part of path /path/*.jpg then nedd add this part to base path
            searchPattern = searchPattern.Replace('\\', '/').TrimStart('/');
            var subDir = NormalizePath(Path.GetDirectoryName(searchPattern));
            if (!string.IsNullOrEmpty(subDir))
            {
                path = path.TrimEnd('/') + "/" + subDir;
                searchPattern = Path.GetFileName(searchPattern);
            }
            var context = new OperationContext();
            var blobItems = new List<IListBlobItem>();
            BlobContinuationToken token = null;
            var operationContext =  new OperationContext();
            if (_directory != null)
            {
                var directoryBlob = _directory;
                if (!string.IsNullOrEmpty(path))
                {
                    directoryBlob = _directory.GetDirectoryReference(path);
                }

                do
                {
                    var resultSegment = await directoryBlob.ListBlobsSegmentedAsync(recursive, BlobListingDetails.Metadata, null, token, _options.BlobRequestOptions, operationContext);
                    token = resultSegment.ContinuationToken;
                    blobItems.AddRange(resultSegment.Results);
                } while (token != null);

            }
            else
            {
                do
                {
                    var resultSegment = await _container.ListBlobsSegmentedAsync(null, recursive, BlobListingDetails.Metadata, null, token, _options.BlobRequestOptions, operationContext);
                    token = resultSegment.ContinuationToken;
                    blobItems.AddRange(resultSegment.Results);
                } while (token != null);

            }
            // Loop over items within the container and output the length and URI.
            foreach (var item in blobItems)
            {
                var block = item as CloudBlockBlob;
                if (block != null)
                {
                    var blobRelativePath = GetRelativeUrl(block.Uri.ToString());
                    var fileName = Path.GetFileName(Uri.UnescapeDataString(block.Uri.ToString()));
                    if (fileName.FitsMask(searchPattern))
                    {
                        retVal.Add(blobRelativePath);
                    }
                }
            }
            return retVal;
        }

        #endregion

        protected virtual string NormalizePath(string path)
        {
            return path.Replace('\\', '/').TrimStart('/');
        }

        protected virtual string GetRelativeUrl(string url)
        {
            var absoluteUrl = GetAbsoluteUrl("");
            return url.Replace(absoluteUrl, string.Empty);
        }

        protected virtual string GetAbsoluteUrl(string path)
        {
            var builder = new UriBuilder(_cloudBlobClient.BaseUri);
            builder.Path += string.Join("/", _options.Container, _options.Directory, path).Replace("//", "/");
            return builder.Uri.ToString();
        }


        protected virtual void RaiseChangedEvent(FileSystemEventArgs args)
        {
            var changedEvent = Changed;
            changedEvent?.Invoke(this, args);
        }

        protected virtual void RaiseRenamedEvent(RenamedEventArgs args)
        {
            var renamedEvent = Renamed;
            renamedEvent?.Invoke(this, args);
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _cancelSource.Cancel();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AzureBlobContentProvider() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
