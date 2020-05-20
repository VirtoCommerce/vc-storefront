using System.IO;
using Markdig;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class StaticContentLoaderFactory : IStaticContentLoaderFactory
    {
        public IStaticContentLoader CreateLoader(ContentItem contentItem)
        {
            switch (Path.GetExtension(contentItem.StoragePath))
            {
                case string value when value.EqualsInvariant(".PAGE"):
                    return new PageBuilderContentLoader();
                case string value when value.EqualsInvariant(".MD"):
                    return new MarkdownContentLoader(new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());
                default:
                    return new StaticContentLoader();
            }
        }
    }
}
