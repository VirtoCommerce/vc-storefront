using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public static class JTokenExtensions
    {
        public static JToken RemovePropertyInChildren(this JToken token, IEnumerable<string> jsonPaths,
            IEnumerable<string> propertyNames)
        {
            if (jsonPaths == null || propertyNames == null)
            {
                return token;
            }

            foreach (var path in jsonPaths)
            {
                var foundTokens = token.SelectTokens(path);
                foreach (var itemToken in foundTokens)
                {
                    foreach (var propertyName in propertyNames)
                    {
                        var foundObject = ((JObject)itemToken);

                        if (!foundObject.ContainsKey(propertyName))
                        {
                            continue;
                        }

                        ((JObject)itemToken).Property(propertyName).Remove();
                    }
                }
            }

            return token;
        }
    }
}
