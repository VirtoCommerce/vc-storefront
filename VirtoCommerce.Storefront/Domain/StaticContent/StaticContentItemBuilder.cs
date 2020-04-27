using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoRest.Core.Utilities;
using Markdig;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder : IStaticContentItemBuilder
    {
        internal static readonly Regex headerRegExp = new Regex(@"(?s:^---(.*?)---)");
        internal static readonly string[] extensions = new[] { ".md", ".liquid", ".html" };

        private readonly IStaticContentItemFactory _factory;

        private readonly List<IContentItemVisitor> _prepareVisitors = new List<IContentItemVisitor>
        {
            new LangVisitor(),
            new YamlMetadataVisitor(),
            new PageMetadataVisitor(),
        };

        private readonly List<IContentItemVisitor> _metadataVisitors = new List<IContentItemVisitor>
        {
            new BlogExcerptMetadataVisitor(),
            new BlogMetadataVisitor(),
            new ContentPageMetadataVisitor(),
            new MetadataVisitor()
        };

        private readonly List<IContentItemVisitor> _contentVisitors = new List<IContentItemVisitor>
        {
            new MarkdownVisitor(new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()),
            new PageContentVisitor(),
        };

        private readonly List<IContentItemVisitor> _postVisitors = new List<IContentItemVisitor> { new UrlsVisitor() };


        public StaticContentItemBuilder(IStaticContentItemFactory factory)
        {
            _factory = factory;
        }

        public ContentItem BuildFrom(string baseStoreContentPath, string blobRelativePath, string content)
        {
            var contentItem = _factory.GetItemFromPath(blobRelativePath);
            var visitedContent = content;
            Action<List<IContentItemVisitor>> runVisitors = visitors =>
            {
                visitors.Where(x => x.Suit(contentItem))
                    .ForEach(x => visitedContent = x.ReadContent(blobRelativePath, visitedContent, contentItem));
            };
            if (contentItem != null)
            {
                contentItem.StoragePath = "/" +
                    (string.IsNullOrEmpty(baseStoreContentPath) ?
                        blobRelativePath :
                        blobRelativePath.Replace(baseStoreContentPath + "/", String.Empty)
                    ).TrimStart('/');
                contentItem.FileName = Path.GetFileName(blobRelativePath);
                new List<List<IContentItemVisitor>>
                    { _prepareVisitors, _metadataVisitors, _contentVisitors, _postVisitors }
                    .ForEach(runVisitors);
            }

            return contentItem;
        }
    }
}
