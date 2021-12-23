using System;
using System.Collections.Generic;
using VirtoCommerce.LiquidThemeEngine;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class StorefrontOptions
    {
        public string DefaultStore { get; set; }

        public string DefaultTheme { get; set; }

        public TimeSpan ChangesPollingInterval { get; set; } = TimeSpan.FromMinutes(1);
        public PlatformEndpointOptions Endpoint { get; set; }
        public LiquidThemeEngineOptions LiquidThemeEngine { get; set; }
        public RequireHttpsOptions RequireHttps { get; set; }
        public bool SendAccountConfirmation { get; set; } = false;
        public int WishlistLimit { get; set; }

        //The options contains mapping of urls with concrete stores
        public IDictionary<string, string> StoreUrls { get; set; } = new Dictionary<string, string>().WithDefaultValue(null);

        public bool CacheEnabled { get; set; }

        public TimeSpan? CacheAbsoluteExpiration { get; set; }
        public TimeSpan? CacheSlidingExpiration { get; set; }

        public int PageSizeMaxValue { get; set; } = 100;

        public string ResetPasswordNotificationGateway { get; set; } = "Email";
        public string TwoFactorAuthenticationNotificationGateway { get; set; } = "Phone";
    }
}
