using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public interface IStaticContentLoader
    {
        void ReadMetaData(string content, IDictionary<string, IEnumerable<string>> metadata);
        string PrepareContent(string content);
    }
}
