using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Stores;
using VirtoCommerce.Tools;

namespace VirtoCommerce.Storefront.Domain
{
    /// <summary>
    /// Represent a search and rendering static content (pages and blogs)
    /// </summary>
    public class StaticContentService : IStaticContentService
    {
        private static readonly string[] _extensions = { ".md", ".liquid", ".html", ".page" };
        private readonly IStaticContentItemFactory _contentItemFactory;
        private readonly IContentBlobProvider _contentBlobProvider;
        private readonly IStorefrontMemoryCache _memoryCache;
        private readonly IStaticContentLoaderFactory _metadataFactory;
        private readonly string _basePath = "Pages";

        public StaticContentService(IStorefrontMemoryCache memoryCache, IStaticContentItemFactory contentItemFactory,
                                        IContentBlobProvider contentBlobProvider, IStaticContentLoaderFactory metadataFactory)
        {
            _contentItemFactory = contentItemFactory;
            _contentBlobProvider = contentBlobProvider;
            _memoryCache = memoryCache;
            _metadataFactory = metadataFactory;
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

                    // each content file  has a name pattern {name}.{language?}.{ext}
                    var localizedBlobs = contentBlobs.Select(x => new LocalizedBlobInfo(x));

                    foreach (var localizedBlob in localizedBlobs.OrderBy(x => x.Name))
                    {
                        var blobRelativePath = "/" + localizedBlob.Path.TrimStart('/');
                        var contentItem = _contentItemFactory.GetItemFromPath(blobRelativePath);
                        if (contentItem != null)
                        {
                            if (contentItem.Name == null)
                            {
                                contentItem.Name = localizedBlob.Name;
                            }

                            contentItem.Language = localizedBlob.Language;
                            contentItem.FileName = Path.GetFileName(blobRelativePath);
                            contentItem.StoragePath = "/" + blobRelativePath.Replace(baseStoreContentPath + "/", string.Empty).TrimStart('/');
                            LoadAndRenderContentItem(blobRelativePath, contentItem);
                            retVal.Add(contentItem);
                        }
                    }
                }

                return retVal.ToArray();
            });
        }

        #endregion

        private void LoadAndRenderContentItem(string contentPath, ContentItem contentItem)
        {
            string content;
            using (var stream = _contentBlobProvider.OpenRead(contentPath))
            {
                // Load raw content with metadata
                content = stream.ReadToString();
            }

            IDictionary<string, IEnumerable<string>> metaHeaders = new Dictionary<string, IEnumerable<string>>();
            var metadataReader = _metadataFactory.CreateLoader(contentItem);

            try
            {
                metadataReader.ReadMetaData(content, metaHeaders);
            }
            catch (Exception ex) // NOTE: Exception must have a specific type!
            {
                var error = $"Failed to parse metadata from \"{contentItem.StoragePath}\"<br/>{ex.Message}";
                content = $"{error}<br/>{content}";
            }

            content = metadataReader.PrepareContent(content);
            contentItem.LoadContent(content, metaHeaders);

            if (string.IsNullOrEmpty(contentItem.Permalink))
            {
                contentItem.Permalink = ":folder/:categories/:title";
            }

            // Transform permalink template to url
            contentItem.Url = GetContentItemUrl(contentItem, contentItem.Permalink);
            // Transform aliases permalink templates to urls
            contentItem.AliasesUrls = contentItem.Aliases.Select(x => GetContentItemUrl(contentItem, x)).ToList();
        }

        private static string GetContentItemUrl(ContentItem item, string permalink)
        {
            return new FrontMatterPermalink
            {
                UrlTemplate = permalink,
                Categories = item.Categories,
                Date = item.CreatedDate,
                FilePath = item.StoragePath
            }.ToUrl();
        }

        // each content file  has a name pattern {name}.{language?}.{ext}
        private class LocalizedBlobInfo
        {
            public LocalizedBlobInfo(string blobPath)
            {
                Path = blobPath;
                Language = Language.InvariantLanguage;

                var parts = System.IO.Path.GetFileName(blobPath)?.Split('.');
                Name = parts?.FirstOrDefault();

                if (parts?.Length == 3)
                {
                    try
                    {
                        Language = new Language(parts[1]);
                    }
                    catch (Exception)
                    {
                        Language = Language.InvariantLanguage;
                    }
                }
            }

            public string Name { get; }
            public Language Language { get; }
            public string Path { get; }
        }
    }
}
