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

        //private void LoadAndRenderContentItem(string contentPath, ContentItem contentItem)
        //{
        //    string content;
        //    using (var stream = _contentBlobProvider.OpenRead(contentPath))
        //    {
        //        // Load raw content with metadata
        //        content = stream.ReadToString();
        //    }

        //    var contentLoader = _staticContentLoaderFactory.CreateLoader(contentItem);
        //    contentLoader.LoadContent(content, contentItem);

        //    if (string.IsNullOrEmpty(contentItem.Permalink))
        //    {
        //        contentItem.Permalink = ":folder/:categories/:title";
        //    }

        //    // Transform permalink template to url
        //    contentItem.Url = GetContentItemUrl(contentItem, contentItem.Permalink);
        //    // Transform aliases permalink templates to urls
        //    contentItem.AliasesUrls = contentItem.Aliases.Select(x => GetContentItemUrl(contentItem, x)).ToList();
        //}

        //private static string GetContentItemUrl(ContentItem item, string permalink)
        //{
        //    return new FrontMatterPermalink
        //    {
        //        UrlTemplate = permalink,
        //        Categories = item.Categories,
        //        Date = item.CreatedDate,
        //        FilePath = item.StoragePath
        //    }.ToUrl();
        //}

        //// each content file  has a name pattern {name}.{language?}.{ext}
        //private class LocalizedBlobInfo
        //{
        //    public LocalizedBlobInfo(string blobPath)
        //    {
        //        Path = blobPath;
        //        Language = Language.InvariantLanguage;

        //        var parts = System.IO.Path.GetFileName(blobPath)?.Split('.');
        //        Name = parts?.FirstOrDefault();

        //        if (parts?.Length == 3)
        //        {
        //            try
        //            {
        //                Language = new Language(parts[1]);
        //            }
        //            catch (Exception)
        //            {
        //                Language = Language.InvariantLanguage;
        //            }
        //        }
        //    }

        //    public string Name { get; }
        //    public Language Language { get; }
        //    public string Path { get; }
        //}
    }
}
