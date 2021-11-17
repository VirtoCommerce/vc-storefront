using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;
using VirtoCommerce.Storefront.Model.Stores;

namespace VirtoCommerce.Storefront.Domain
{
    public static class SelectCurrentCurrencyPolicy
    {
        public static Currency GetCurrentCurrency(this HttpContext context, IEnumerable<Currency> currencies, Store store)
        {
            //Try get default store currency first
            var result = currencies.FirstOrDefault(x => x.Equals(store.DefaultCurrencyCode));
            //Try get currency from request url  
            StringValues currencyCode;
            if (!context.Request.Query.TryGetValue("currency", out currencyCode))
            {
                //Next try get from claims
                currencyCode = context.User.FindFirstValue(SecurityConstants.Claims.CurrencyClaimType);
            }
            //Get store default currency if currency not in the supported by stores list
            if (!string.IsNullOrEmpty(currencyCode))
            {
                result = currencies.FirstOrDefault(x => x.Equals(currencyCode.First())) ?? result;
            }
            return result;
        }

    }
}
