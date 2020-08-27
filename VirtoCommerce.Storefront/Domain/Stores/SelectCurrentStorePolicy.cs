using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SelectCurrentStorePolicy
    {
        public static Store GetCurrentStore(this HttpContext context, IEnumerable<Store> stores, string defaultStoreId)
        {      
            if(stores == null)
            {
                throw new ArgumentNullException(nameof(stores));
            }

            //Try first find by store url (if it defined)
            var result = stores.FirstOrDefault(x => context.Request.Path.StartsWithSegments(new PathString("/" + x.Id)));

            if (result == null)
            {
                result = stores.FirstOrDefault(x => x.IsStoreUrl(context.Request.GetUri()));
            }
            if (result == null && defaultStoreId != null)
            {
                result = stores.FirstOrDefault(x => x.Id.EqualsInvariant(defaultStoreId));
            }
            if(result == null)
            {
                result = stores.FirstOrDefault();
            }

            return result;
        }

        public static void ReplaceThemeForPreviewIfRequired(this Store store, HttpContext context)
        {
            if (context.Request.QueryString.HasValue)
            {
                var themePreview = QueryHelpers.ParseQuery(context.Request.QueryString.Value);

                if (store != null && themePreview.ContainsKey("previewtheme"))
                {
                    var previewTheme = themePreview["previewtheme"].FirstOrDefault();

                    store.ThemeName = previewTheme;
                }
            }
        }
    }
}
