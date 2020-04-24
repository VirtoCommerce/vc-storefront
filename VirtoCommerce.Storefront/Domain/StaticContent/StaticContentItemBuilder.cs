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
        private static readonly Regex _headerRegExp = new Regex(@"(?s:^---(.*?)---)");
        private static readonly string[] _extensions = new[] { ".md", ".liquid", ".html" };

        private readonly IStaticContentItemFactory _factory;

        private readonly List<IContentItemParser> _prepareParsers = new List<IContentItemParser>
        {
            new LangParser(),
            new YamlMetadataParser(),
            new PageMetadataParser(),
        };

        private readonly List<IContentItemParser> _metadataParsers = new List<IContentItemParser>
        {
            new BlogExcerptMetadataParser(),
            new BlogMetadataParser(),
            new ContentPageMetadataParser(),
            new MetadataParser()
        };

        private readonly List<IContentItemParser> _contentParsers = new List<IContentItemParser>
        {
            new ContentWithYamlParser(new MarkdownPipelineBuilder().UseAdvancedExtensions().Build()),
            new PageContentParser(),
        };

        private readonly List<IContentItemParser> _postParsers = new List<IContentItemParser> { new UrlsParser() };


        public StaticContentItemBuilder(IStaticContentItemFactory factory)
        {
            _factory = factory;
        }

        public ContentItem BuildFrom(string baseStoreContentPath, string blobRelativePath, string content)
        {
            var contentItem = _factory.GetItemFromPath(blobRelativePath);
            Action<List<IContentItemParser>> runParsers = parsers =>
            {
                parsers.Where(x => x.Suit(contentItem))
                    .ForEach(x => x.Parse(blobRelativePath, content, contentItem));
            };
            if (contentItem != null)
            {
                contentItem.StoragePath = "/" + blobRelativePath.Replace(baseStoreContentPath + "/", String.Empty).TrimStart('/');
                contentItem.FileName = Path.GetFileName(blobRelativePath);
                new List<List<IContentItemParser>>
                    { _prepareParsers, _metadataParsers, _contentParsers, _postParsers }
                    .ForEach(runParsers);
            }

            return contentItem;
        }

        private interface IContentItemParser
        {
            bool Suit(ContentItem item);
            void Parse(string path, string content, ContentItem item);
        }
    }
}
