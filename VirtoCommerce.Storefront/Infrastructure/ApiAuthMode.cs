using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public enum ApiAuthMode
    {
        /// <summary>
        ///  Barrier token by using AppId and SecretKey
        /// </summary>
        BarrierToken,
        /// <summary>
        /// OAuth2 resource owner password credentials  grand flow (username, password)
        /// </summary>
        OAuthPassword
    }
}
