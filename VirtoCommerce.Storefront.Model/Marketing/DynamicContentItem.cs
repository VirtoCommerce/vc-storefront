using System;
using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    public partial class DynamicContentItem : Entity
    {
        public DynamicContentItem()
        {
            DynamicProperties = new List<DynamicProperty>();
        }

        public string ContentType { get; set; }

        public string Description { get; set; }

        public string FolderId { get; set; }

        public string Name { get; set; }

        public string ObjectType { get; set; }

        public string Outline { get; set; }

        public string Path { get; set; }


        public IList<DynamicProperty> DynamicProperties { get; set; }
    }
}
