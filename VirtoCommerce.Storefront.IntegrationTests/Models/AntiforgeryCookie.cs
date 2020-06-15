using System;
using System.Linq;
using System.Net.Http.Headers;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.IntegrationTests.Models
{
    public class AntiforgeryCookie
    {
        public string Key => "X-XSRF-TOKEN";
        public string Value { get; }

        public AntiforgeryCookie(HttpHeaders headers)
        {

            var cookies = headers.FirstOrDefault(c => c.Key.Contains("Cookie")).Value.ToList();
            if (cookies.IsNullOrEmpty())
            {
                throw new NullReferenceException("Cookies not found");
            }

            var antiforgeryRawValue = cookies.FirstOrDefault(c => c.Contains("XSRF-TOKEN"));
            if (antiforgeryRawValue == null)
            {
                throw new NullReferenceException("Antiforgery cookie not found");
            }

            Value = antiforgeryRawValue.Split(";")[0].Split("=")[1];
        }

    }
}
