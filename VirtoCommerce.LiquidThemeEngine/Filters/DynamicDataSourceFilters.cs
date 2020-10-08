using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using GraphQL;
using Scriban;
using Scriban.Runtime;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public static class DataSourceFilter
    {
        public static object GraphqlDataSource(TemplateContext context, string fileName)
        {
            //TODO: replace all @placeholders to values from context using regexp

            var themeAdaptor = (ShopifyLiquidThemeEngine)context.TemplateLoader;
            var query = themeAdaptor.GetAssetStreamAsync(Path.Combine("graphql", fileName)).GetAwaiter().GetResult().ReadToString();
            var graphQLRequest = new GraphQLRequest(query);
            var response = themeAdaptor.GraphQLClient.SendQueryAsync<ExpandoObject>(graphQLRequest).GetAwaiter().GetResult();
            var result = BuildScriptObject(response.Data);
            return result;
        }

        private static ScriptObject BuildScriptObject(ExpandoObject expando)
        {
            var dict = (IDictionary<string, object>)expando;
            var scriptObject = new ScriptObject();

            foreach (var kv in dict)
            {
                var renamedKey = GraphQLMemberRenamer.Rename(kv.Key);

                if (kv.Value is ExpandoObject expandoValue)
                {
                    scriptObject.Add(renamedKey, BuildScriptObject(expandoValue));
                }
                else if (kv.Value is IList array)
                {
                    var firstValue = array.Count > 0 ? array[0] : null;
                    if (firstValue is ExpandoObject expandoObj)
                    {
                        scriptObject.Add(renamedKey, array.OfType<ExpandoObject>().Select(x => BuildScriptObject(x)).ToArray());
                    }
                    else
                    {
                        scriptObject.Add(renamedKey, array);
                    }
                }
                else
                {
                    scriptObject.Add(renamedKey, kv.Value);
                }
            }

            return scriptObject;
        }

        private class GraphQLMemberRenamer
        {
            protected GraphQLMemberRenamer()
            {
            }
            /// <summary>
            /// Renames a camel/pascalcase member to a lowercase and `_` name. e.g `ThisIsAnExample` becomes `this_is_an_example`.
            /// </summary>
            /// <param name="member">The member to rename</param>
            /// <returns>The member name renamed</returns>
            public static string Rename(string name)
            {
                var builder = new StringBuilder();
                bool previousUpper = false;
                for (int i = 0; i < name.Length; i++)
                {
                    var c = name[i];
                    if (char.IsUpper(c))
                    {
                        if (i > 0 && !previousUpper)
                        {
                            builder.Append("_");
                        }
                        builder.Append(char.ToLowerInvariant(c));
                        previousUpper = true;
                    }
                    else
                    {
                        builder.Append(c);
                        previousUpper = false;
                    }
                }
                return builder.ToString();
            }
        }
    }
}
