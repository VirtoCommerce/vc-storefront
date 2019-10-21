using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.StaticContent
{
    public interface IMetaDataLoader
    {
        void ReadMetaData(string content, IDictionary<string, IEnumerable<string>> metadata);
    }
}
