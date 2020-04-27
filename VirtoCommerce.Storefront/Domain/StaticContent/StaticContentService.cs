using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    /// <summary>
    /// Represent a search and rendering static content (pages and blogs)
    /// </summary>
    public class StaticContentService : IStaticContentService
    {
        private static readonly string[] _extensions = { ".md", ".liquid", ".html", ".page" };
        private readonly IContentBlobProvider _contentBlobProvider;
        private readonly IStaticContentItemBuilder _builder;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly string _basePath = "Pages";

        public StaticContentService(IStorefrontMemoryCache memoryCache, IContentBlobProvider contentBlobProvider, IStaticContentItemBuilder builder)
        {
            _contentBlobProvider = contentBlobProvider;
            _builder = builder;
            _memoryCache = memoryCache;
        }

        #region IStaticContentService Members

        public IEnumerable<ContentItem> LoadStoreStaticContent(Store store)
        {
            var baseStoreContentPath = string.Concat(_basePath, "/", store.Id);
            var cacheKey = CacheKey.With(GetType(), "LoadStoreStaticContent", store.Id);
            return _memoryCache.GetOrCreateExclusive(cacheKey, (cacheEntry) =>
            {
                cacheEntry.AddExpirationToken(new CompositeChangeToken(new[] { StaticContentCacheRegion.CreateChangeToken(), _contentBlobProvider.Watch(baseStoreContentPath + "/**/*") }));

                var retVal = new List<ContentItem>();
                const string searchPattern = "*.*";

                if (_contentBlobProvider.PathExists(baseStoreContentPath))
                {

                    // Search files by requested search pattern
                    var contentBlobs = _contentBlobProvider.Search(baseStoreContentPath, searchPattern, true)
                                                 .Where(x => _extensions.Any(x.EndsWith))
                                                 .Select(x => x.Replace("\\\\", "\\"));

                    foreach (var contentBlob in contentBlobs)
                    {
                        var blobRelativePath = "/" + contentBlob.TrimStart('/');

                        var contentItem = _builder.BuildFrom(baseStoreContentPath, blobRelativePath, GetContent(blobRelativePath));
                        if (contentItem != null)
                        {
                            retVal.Add(contentItem);
                        }
                    }
                }

                return retVal.ToArray();
            });
        }

        private string GetContent(string contentPath)
        {
            string result;
            using (var stream = _contentBlobProvider.OpenRead(contentPath))
            {
                // Load raw content with metadata
                result = stream.ReadToString();
            }

            return result;
        }

        #endregion
    }
}
