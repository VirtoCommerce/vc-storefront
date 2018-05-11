using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public interface IStaticContentItemFactory
    {
        ContentItem GetItemFromPath(string path);
    }
}
