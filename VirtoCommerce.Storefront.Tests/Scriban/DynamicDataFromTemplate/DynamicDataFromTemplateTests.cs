using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using GraphQL.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Scriban;
using Scriban.Runtime;
using VirtoCommerce.Storefront.Model.Common;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Scriban
{
    public class DynamicDataFromTemplateTests
    {
        [Fact]
        public void DynamicDataBinding()
        {
            //Define complex data_sources (nested and use liquid expressions in the data sources definitions)???
            //Load data source queries from separated files or settings
            //How to pass parameters into data sources definitions. use @interpolation
            //IAccessibleByIndexKey for dynamic script objects to be able accessing  by key from liquid templates?
            //Diagnostics -  graphQL errors handling, schema mismatch
            //Required essential  workcontext variables (objects) (User, Language, Store, Request, Slug, Category.Path, Breadcrumbs)
            //Theme incompatible (need to convert odt theme to using the new version storefront).
            //Need to detect and eliminate all model changes. Need keep in mind that pages that will be used SSR will be very limited, only essential for SEO is required
            //GraphQL interfaces and abstract types for aggregations for aggregations

            //The main question is: Completely backward incompatibility with previous objects naming and structure, migrate ODT theme to the new version.
            var liquidTemplate = @"
             {% dataSource1 = '{ products( filter: categories.subtree:@outline) { totalCount items { id name type images { url } } } }' | graphql_data_source %}
             {% dataSource2 = 'products.graphql' | graphql_data_source %}
             {% for myCoolProduct in dataSource1.products.items %}
                 {{ myCoolProduct.name }}
                 {% if myCoolProduct.type == 'digital' %}
                    {{ myCoolProduct.images[0].url }}
                 {% endif %}
             {% endfor %}
             ";

            var parsedTemplate = Template.ParseLiquid(liquidTemplate);      
   
            var context = new LiquidTemplateContext();
            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(DataSourceFilter));
            context.PushGlobal(scriptObject);
            //First render with fake data
            var result = parsedTemplate.Render(context);
        }

        public static class DataSourceFilter
        {
            public static object GraphqlDataSource(TemplateContext context, string query)
            {
               //TODO: replace all @placeholders to values from context using regexp
            

                var json = @"
{
    ""data"": {
        ""products"": {
            ""totalCount"": 1,
            ""items"": [
                {
                    ""id"": 1,
                    ""name"": ""my-cool-product"",
                    ""type"": ""digital"",
                    ""images"": [
                        {
                            ""url"": ""http://localhost/123""
                        }
                    ]
                }
            ]
        }
    }
}";
                var expando = JsonConvert.DeserializeObject<ExpandoObject>(json);
                var result = BuildScriptObject(((IDictionary<string, object>)expando)["data"] as ExpandoObject);
                return result;
            }
        }

        private static ScriptObject BuildScriptObject(ExpandoObject expando)
        {
            var dict = (IDictionary<string, object>)expando;
            var scriptObject = new ScriptObject();

            foreach (var kv in dict)
            {
                var renamedKey = StandardMemberRenamer.Rename(kv.Key);

                if (kv.Value is ExpandoObject expandoValue)
                {
                    scriptObject.Add(renamedKey, BuildScriptObject(expandoValue));
                }
                else if (kv.Value is IList array)
                {
                    var firstValue = array.Count > 0 ? array[0] : null;
                    if(firstValue is ExpandoObject expandoObj)
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

       

        public sealed class StandardMemberRenamer
        {
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
