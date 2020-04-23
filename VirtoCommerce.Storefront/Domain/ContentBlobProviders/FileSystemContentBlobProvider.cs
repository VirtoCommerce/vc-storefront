using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Caching;
using VirtoCommerce.Storefront.Extensions;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class FileSystemContentBlobProvider : IContentBlobProvider
    {
        private readonly FileSystemBlobContentOptions _options;
        private readonly IStorefrontMemoryCache _memoryCache;
        // Keep links to file watchers to prevent GC to collect it
        private readonly PhysicalFilesWatcher _fileSystemWatcher;

        public FileSystemContentBlobProvider(IOptions<FileSystemBlobContentOptions> options, IStorefrontMemoryCache memoryCache)
        {
            _options = options.Value;
            _memoryCache = memoryCache;
            //Create fileSystemWatcher instance only when rootFolder exist to prevent whole application crash on initialization phase. 
            if (Directory.Exists(_options.Path))
            {
                //It is very important to have rootPath with leading slash '\' without this any changes won't reflected
                var rootPath = _options.Path.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
                _fileSystemWatcher = new PhysicalFilesWatcher(rootPath, new FileSystemWatcher(rootPath), false);
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
            path = NormalizePath(path);
            // traversing above root not permitted.
            if (PathUtils.PathNavigatesAboveRoot(path))
            {
                throw new InvalidOperationException(path);
            }
            return File.OpenRead(path);
        }

        /// <summary>
        /// Open blob for write by path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>blob stream</returns>
        public virtual Stream OpenWrite(string path)
        {
            path = NormalizePath(path);
            // traversing above root not permitted.
            if (PathUtils.PathNavigatesAboveRoot(path))
            {
                throw new InvalidOperationException(path);
            }

            var folderPath = Path.GetDirectoryName(path);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return File.Open(path, FileMode.Create);
        }

        /// <summary>
        /// Check that blob or folder with passed path exist
        /// </summary>
        /// <param name="path">relative path /folder/blob.md</param>
        /// <returns></returns>
        public virtual bool PathExists(string path)
        {
            var cacheKey = CacheKey.With(GetType(), "PathExists", path);
            return _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                path = NormalizePath(path);
                cacheEntry.AddExpirationToken(Watch(path));
                cacheEntry.AddExpirationToken(ContentBlobCacheRegion.CreateChangeToken());
                var retVal = Directory.Exists(path);
                if (!retVal)
                {
                    retVal = File.Exists(path);
                }
                return retVal;
            });
        }

        /// <summary>
        /// Search blob content in specified folder
        /// </summary>
        /// <param name="path">relative folder path in which the search  Example: /folder</param>
        /// <param name="searchPattern">search blob name pattern can be used mask (*, ? symbols)</param>
        /// <param name="recursive"> recursive search</param>
        /// <returns>Returns relative path for all found blobs  example: /folder/blob.md </returns>
        public virtual IEnumerable<string> Search(string path, string searchPattern, bool recursive)
        {
            var retVal = new List<string>();
            path = NormalizePath(path);
            searchPattern = searchPattern.TrimStart(Path.PathSeparator);
            if (Directory.Exists(Path.GetDirectoryName(Path.Combine(path, searchPattern))))
            {
                var files = Directory.GetFiles(path, searchPattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                                     .Select(x => GetRelativePath(x));
                retVal.AddRange(files);
            }
            return retVal;
        }

        public virtual IChangeToken Watch(string path)
        {
            //TODO: Test with symbolic links
            if (_fileSystemWatcher != null)
            {
                // Absolute paths not permitted for watcher, need to convert it to relative
                if (Path.IsPathRooted(path))
                {
                    path = GetRelativePath(path).TrimStart('/');
                }
                return _fileSystemWatcher.CreateFileChangeToken(path);
            }
            else
            {
                return new CancellationChangeToken(new CancellationToken());
            }
        }
        #endregion

        protected virtual string GetRelativePath(string path)
        {
            return path.Replace(_options.Path, string.Empty).Replace(Path.DirectorySeparatorChar, '/');
        }

        protected virtual string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            path = path.Replace('/', Path.DirectorySeparatorChar);
            path = path.Replace(_options.Path, string.Empty);
            return Path.Combine(_options.Path, path.TrimStart(Path.DirectorySeparatorChar));
        }


    }
}
