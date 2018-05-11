using System;
using Microsoft.Rest;

namespace VirtoCommerce.Storefront.Common
{
    public static class ServiceClientExtensions
    {
        public static T DisableRetries<T>(this T client)
            where T : ServiceClient<T>
        {
            client.SetRetryPolicy(null);
            return client;
        }

        public static T WithTimeout<T>(this T client, TimeSpan timeout)
            where T : ServiceClient<T>
        {
            client.HttpClient.Timeout = timeout;
            return client;
        }
    }
}
