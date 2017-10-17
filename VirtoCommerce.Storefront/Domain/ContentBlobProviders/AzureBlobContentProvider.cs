using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class AzureBlobContentProvider : IContentBlobProvider
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly CloudBlobContainer _container;
        private readonly CloudBlobDirectory _directory;
        private readonly IMemoryCache _memoryCache;
        private readonly AzureBlobContentOptions _options;

        public AzureBlobContentProvider(IOptions<AzureBlobContentOptions> options, IMemoryCache memoryCache)
        {
            _options = options.Value;
            _memoryCache = memoryCache;       

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
        }       

        #region IContentBlobProvider Members
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
            var cacheKey = CacheKey.With(GetType(), "PathExistsAsync", path);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey,  async (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(ContentBlobCacheRegion.CreateChangeToken());

                // If requested path is a directory we should always return true because Azure blob storage does not support checking if directories exist
                var result = string.IsNullOrEmpty(Path.GetExtension(path));
                if (!result)
                {
                    var url = GetAbsoluteUrl(path);
                    try
                    {
                        result = await (await _cloudBlobClient.GetBlobReferenceFromServerAsync(new Uri(url))).ExistsAsync();
                    }
                    catch (Exception)
                    {
                        //Azure blob storage client does not provide method to check blob url exist without throwing exception
                    }
                }
                return result;
            });
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

        public virtual IChangeToken Watch(string path)
        {
            //TODO
            //See https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-blob
            return new CancellationChangeToken(new CancellationToken());
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
    }
}
