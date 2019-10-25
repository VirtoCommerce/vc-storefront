using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public interface IStaticContentLoaderFactory
    {
        IStaticContentLoader CreateLoader(ContentItem contentItem);
    }
}
