using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.Storefront.Model.Security
{
    public class ExternalUserLoginInfo
    {
     
        //  Examples of the provider may be Local, Facebook, Google, etc.
        public string LoginProvider { get; set; }
        //
        // Summary:
        //     Gets or sets the unique identifier for the user identity user provided by the
        //     login provider.
        //
        // Remarks:
        //     This would be unique per provider, examples may be @microsoft as a Twitter provider
        //     key.
        public string ProviderKey { get; set; }
        //
        // Summary:
        //     Gets or sets the display name for the provider.
        public string ProviderDisplayName { get; set; }
    }
}
