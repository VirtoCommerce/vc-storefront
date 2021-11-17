using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Common.Exceptions;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class AzureBlobContentProvider : IContentBlobProvider
    {
        private readonly CloudBlobClient _cloudBlobClient;
        private readonly CloudBlobContainer _container;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly AzureBlobContentOptions _options;
        private readonly IBlobChangesWatcher _watcher;

        public AzureBlobContentProvider(IOptions<AzureBlobContentOptions> options, IStorefrontMemoryCache memoryCache, IBlobChangesWatcher watcher)
        {
            _options = options.Value;
            _memoryCache = memoryCache;

            if (!CloudStorageAccount.TryParse(_options.ConnectionString, out var cloudStorageAccount))
            {
                throw new StorefrontException("Failed to get valid connection string");
            }
            _cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            _container = _cloudBlobClient.GetContainerReference(_options.Container);
            _watcher = watcher;
        }

        #region IContentBlobProvider Members
        /// <summary>
        /// Open blob for read 
        /// </summary>
        /// <param name="path">blob relative path /folder/blob.md</param>
        /// <returns></returns>
        public virtual Stream OpenRead(string path)
        {
            return OpenReadAsync(path).GetAwaiter().GetResult();
        }

        public virtual Task<Stream> OpenReadAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            path = NormalizePath(path);

            return OpenReadInternalAsync(path);
        }

        protected virtual async Task<Stream> OpenReadInternalAsync(string path)
        {
            path = NormalizePath(path);

            Stream result = null;
            try
            {
                result = await _container.GetBlobReference(path).OpenReadAsync();
            }
            catch (Exception)
            {
                //we should not throw an exception for the missed directories and return null as result, because the Azure blob storage does not allow us to check if directories exist
                //and PathExists method will always return true for these paths.                
                if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                {
                    //Throw the not found exception for files
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// Open blob for write by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>blob stream</returns>
        public virtual Stream OpenWrite(string path)
        {
            return OpenWriteAsync(path).GetAwaiter().GetResult();
        }

        public virtual async Task<Stream> OpenWriteAsync(string path)
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
            return PathExistsAsync(path).GetAwaiter().GetResult();
        }

        public virtual async Task<bool> PathExistsAsync(string path)
        {
            path = NormalizePath(path);
            var cacheKey = CacheKey.With(GetType(), "PathExistsAsync", path);
            return await _memoryCache.GetOrCreateExclusiveAsync(cacheKey, async (cacheEntry) =>
           {
               cacheEntry.AddExpirationToken(ContentBlobCacheRegion.CreateChangeToken());

               var isDirectory = string.IsNullOrEmpty(Path.GetExtension(path));
               var result = false;

               if (isDirectory)
               {
                   //There is only one way to check if the blob directory exists is to request its contents.
                   BlobContinuationToken token = null;
                   var operationContext = new OperationContext();
                   var directory = GetCloudBlobDirectory(path);
                   var resultSegment = await directory.ListBlobsSegmentedAsync(true, BlobListingDetails.Metadata, 1, token, _options.BlobRequestOptions, operationContext);
                   result = resultSegment.Results.Any();
               }
               else
               {
                   try
                   {
                       var url = GetAbsoluteUrl(path);
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
            return SearchAsync(path, searchPattern, recursive).GetAwaiter().GetResult();
        }

        public virtual async Task<IEnumerable<string>> SearchAsync(string path, string searchPattern, bool recursive)
        {
            var retVal = new List<string>();
            path = NormalizePath(path);
            //Search pattern may contains a part of path /path/*.jpg then need to add this part to a base path
            searchPattern = searchPattern.Replace('\\', '/').TrimStart('/');
            var subDir = NormalizePath(Path.GetDirectoryName(searchPattern));
            if (!string.IsNullOrEmpty(subDir))
            {
                path = path.TrimEnd('/') + "/" + subDir;
                searchPattern = Path.GetFileName(searchPattern);
            }

            //Try to check that passed search pattern doesn't contain mask wildcard characters
            //this means that a direct link to the resource is passed, and we do not need to perform any search
            var directPath = Path.Combine(path, searchPattern);
            if (!searchPattern.FilePathHasMaskChars() && await PathExistsAsync(directPath))
            {
                retVal.Add(directPath);
            }
            else
            {
                var blobItems = new List<IListBlobItem>();
                BlobContinuationToken token = null;
                var operationContext = new OperationContext();
                var directory = GetCloudBlobDirectory(path);
                do
                {
                    var resultSegment = await directory.ListBlobsSegmentedAsync(recursive, BlobListingDetails.Metadata, null, token, _options.BlobRequestOptions, operationContext);
                    token = resultSegment.ContinuationToken;
                    blobItems.AddRange(resultSegment.Results);
                } while (token != null);

                // Loop over items within the container and output the length and URI.
                foreach (var item in blobItems)
                {
                    if (item is CloudBlockBlob block)
                    {
                        var blobRelativePath = GetRelativeUrl(block.Uri.ToString());
                        var fileName = Path.GetFileName(Uri.UnescapeDataString(block.Uri.ToString()));
                        if (fileName.FitsMask(searchPattern))
                        {
                            retVal.Add(blobRelativePath);
                        }
                    }
                }
            }
            return retVal;
        }

        public virtual IChangeToken Watch(string path)
        {
            return _watcher.CreateBlobChangeToken(NormalizePath(path));
        }
        #endregion

        protected virtual CloudBlobDirectory GetCloudBlobDirectory(string path)
        {
            var isPathToFile = !string.IsNullOrEmpty(Path.GetExtension(path));
            if (isPathToFile)
            {
                path = NormalizePath(Path.GetDirectoryName(path));
            }
            return _container.GetDirectoryReference(path);
        }

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
            builder.Path += string.Join("/", _options.Container, path).Replace("//", "/");
            return builder.Uri.ToString();
        }
    }
}
