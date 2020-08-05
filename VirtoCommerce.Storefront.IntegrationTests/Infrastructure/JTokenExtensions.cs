using System.Collections.Generic;
using System.Linq;
using AutoRest.Core.Utilities;
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

                var foundTokens = token.SelectTokens(path).ToArray();

                if (foundTokens.IsNullOrEmpty() && token is JArray arrayToken)
                {
                    foundTokens = arrayToken.SelectMany(x => x.SelectTokens(path)).ToArray();
                }

                FindAndRemoveTokens(foundTokens, propertyNames.ToArray());
            }

            return token;
        }

        private static void FindAndRemoveTokens(IEnumerable<JToken> foundTokens, IList<string> propertyNames)
        {
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

        private static void TryRemoveProperty(JToken @object, string propertyName)
        {
            if (propertyName.Contains('.'))
            {
                var splitted = propertyName.Split('.');
                var pathToSearch = splitted.FirstOrDefault() ?? string.Empty;
                var propertyToDelete = splitted.Skip(1).FirstOrDefault() ?? string.Empty;

                if (@object is JObject jObject1 && jObject1.ContainsKey(pathToSearch))
                {
                    FindAndRemoveTokens(jObject1.SelectTokens(pathToSearch), new [] { propertyToDelete });
                }
            }

            if (@object is JObject jObject && jObject.ContainsKey(propertyName))
            {
                jObject.Property(propertyName)?.Remove();
            }
        }
    }
}
