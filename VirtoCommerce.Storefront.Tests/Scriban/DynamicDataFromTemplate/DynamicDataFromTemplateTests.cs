using System.Diagnostics;
using GraphQL.Types;
using Scriban;
using Scriban.Runtime;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Scriban
{
    public class DynamicDataFromTemplateTests
    {


        [Fact]
        public void DynamicDataBinding()
        {
            var liquidTemplate = @"
             {% for myCoolProduct in schema.products %}
                 {{ myCoolProduct.name }}
                 {% if myCoolProduct.type == 'digital' %}
                    {{ myCoolProduct.images[0].url }}
                 {% endif %}
             {% endfor %}
             ";

            var schemaDefinition = @"
                type Image {
                  url: String
                }
                type Product {
                  id: ID!
                  name: String
                  images: [Image]
                }

                type Query {
                  products: [Product]                
                }
            ";

            var schema = Schema.For(schemaDefinition);
            
            var parsedTemplate = Template.ParseLiquid(liquidTemplate);

            var dataRecorder = DynamicUsedDataRecorder.From(schema);
     
            var context = new TemplateContext(dataRecorder);

            //First render with fake data
            parsedTemplate.Render(context);

            var allUsedData = dataRecorder.GetAllUsedData();

            Assert.Equal(@"products { name type images { url } }", allUsedData);

            //Call with resulting query to data source
            var dynamicScriptObj = DynamicDataScriptObject.From(schema, "json with graphql result respose graph");

            context = new TemplateContext(dynamicScriptObj);
            var result = parsedTemplate.Render(context);


        }

        public class DynamicDataScriptObject : ScriptObject
        {
            public static DynamicDataScriptObject From(ISchema schema, string objGraph)
            {
                return new DynamicDataScriptObject();
            }
        }

        public class DynamicUsedDataRecorder : ScriptObject
        {
            public string GetAllUsedData()
            {
                return "";
            }
            public static DynamicUsedDataRecorder From(ISchema schema)
            {
                //Parse schema and construct inner properties according to schema AST
                return new DynamicUsedDataRecorder();
            }
        }

    }
      
}
