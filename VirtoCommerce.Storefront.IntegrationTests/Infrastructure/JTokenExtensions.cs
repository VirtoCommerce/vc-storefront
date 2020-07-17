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
                        switch (itemToken)
                        {
                            case JObject foundObject:
                                TryRemoveProperty(foundObject, propertyName);
                                break;
                            case JArray arrayObject:
                            {
                                foreach (var obj in arrayObject)
                                {
                                    TryRemoveProperty(obj, propertyName);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return token;
        }

        private static void TryRemoveProperty(JToken @object, string propertyName)
        {
            if (@object is JObject jObject && jObject.ContainsKey(propertyName))
            {
                jObject.Property(propertyName)?.Remove();
            }
        }
    }
}
