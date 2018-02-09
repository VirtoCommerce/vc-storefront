using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;
using VirtoCommerce.Storefront.Model.Cms;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Cms
{
    public class CmsContentPage : ContentPage
    {
        public CmsPageDefinition CmsPage { get; set; }

        public CmsContentPage()
            : base()
        {
            CmsPage = new CmsPageDefinition();
        }
    }
}
