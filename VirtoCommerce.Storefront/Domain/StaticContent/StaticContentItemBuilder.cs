using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AutoRest.Core.Utilities;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder : IStaticContentItemBuilder
    {
        internal static readonly Regex headerRegExp = new Regex(@"(?s:^---(.*?)---)");
        internal static readonly string[] extensions = new[] { ".md", ".liquid", ".html" };

        private readonly IStaticContentItemFactory _factory;
        private readonly IEnumerable<IContentItemVisitor> _visitors;

        public StaticContentItemBuilder(IStaticContentItemFactory factory, IEnumerable<IContentItemVisitor> visitors)
        {
            _factory = factory;
            _visitors = visitors;
        }

        public ContentItem BuildFrom(string baseStoreContentPath, string blobRelativePath, string content)
        {
            var contentItem = _factory.GetItemFromPath(blobRelativePath);
            if (contentItem != null)
            {
                contentItem.StoragePath = "/" +
                    (string.IsNullOrEmpty(baseStoreContentPath) ?
                        blobRelativePath :
                        blobRelativePath.Replace(baseStoreContentPath + "/", String.Empty)
                    ).TrimStart('/');
                contentItem.FileName = Path.GetFileName(blobRelativePath);
                var visitedContent = content;
                _visitors.OrderBy(x => x.Stage).Where(x => x.Suit(contentItem))
                    .ForEach(x => visitedContent = x.ReadContent(blobRelativePath, visitedContent, contentItem));
            }

            return contentItem;
        }
    }
}
