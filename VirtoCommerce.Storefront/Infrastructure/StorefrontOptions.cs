using System;
using System.Collections.Generic;
using VirtoCommerce.LiquidThemeEngine;

namespace VirtoCommerce.Storefront.Infrastructure
{
    public class StorefrontOptions
    {
        public string DefaultStore { get; set; }
        public TimeSpan ChangesPoolingInterval { get; set; } = TimeSpan.FromMinutes(1);
        public PlatformEndpointOptions Endpoint { get; set; }
        public LiquidThemeEngineOptions LiquidThemeEngine { get; set; }
        public RequireHttpsOptions RequireHttps { get; set; }
        public bool SendAccountConfirmation { get; set; } =  false;
        public int WishlistLimit { get; set; }
    }
}
