using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string FindFirstValue(this ClaimsPrincipal principal, string[] claimTypes, string defaultValue )
        {
            foreach (var claimType in claimTypes)
            {
                var result = principal.FindFirstValue(claimType);
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return defaultValue;
        }
    }
}
