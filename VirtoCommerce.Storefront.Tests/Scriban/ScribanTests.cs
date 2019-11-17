using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
using VirtoCommerce.LiquidThemeEngine.Scriban;
using VirtoCommerce.Storefront.Model.Catalog;
using VirtoCommerce.Storefront.Model.Common;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Scriban
{
    public class ScribanTests
    {
        /// <summary>
        /// https://github.com/lunet-io/scriban/issues/102
        /// </summary>
        [Fact]
        public void Parse_EmptyRawBlock_IsCorrect()
        {
            // arrange
            var template = "{% raw %}{% endraw %}{{ some_var }}";
            // act
            var parsedTemplate = Template.ParseLiquid(template);
            // assert
            Assert.False(parsedTemplate.HasErrors);
        }

        [Fact]
        public void Parse_CaptureWithElsif_IsCorrect()
        {
            // arrange
            var template = "{% capture tag_label_template %}tags.{{ tag.group_type }}.{% if tag.lower and tag.upper %}between{% elsif tag.lower %}greater{% elsif tag.upper %}less{% endif %}{% endcapture %}{{ tag_label_template }}";
            // act
            var parsedTemplate = Template.ParseLiquid(template);
            // assert
            Assert.False(parsedTemplate.HasErrors);
        }

        [Fact]
        public void Render_Capture_With_Elsif_IsCorrect()
        {
            //arrange
            var template = "{% capture tag_label_template %}tags.{{ tag.group_type }}.{% if tag.lower and tag.upper %}between{% elsif tag.lower %}greater{% elsif tag.upper %}less{% endif %}{% endcapture %}{{ tag_label_template }}";
            var scriptObject = new ScriptObject();
            scriptObject.SetValue("tag", new Tag { GroupType = "pricerange", Lower = "11$" }, true);
            var context = new TemplateContext();

            // act
            var parsedTemplate = Template.ParseLiquid(template);                                  
            context.PushGlobal(scriptObject);
            var result = parsedTemplate.Render(context);

            // assert
            Assert.Equal("tags.pricerange.greater", result);
        }

        [Fact]
        public void Parse_FunctionWithContextAndParams_IsCorrect()
        {
            // arrange
            var template = "{{ '{0}{1}' | t: '1', '1'  }}";
            // act
            var parsedTemplate = Template.ParseLiquid(template);
            // assert
            Assert.False(parsedTemplate.HasErrors);
        }

        [Fact]
        public void Render_FunctionWithContextAndParams_IsCorrect()
        {
            // arrange
            var parsedTemplate = Template.ParseLiquid("{{ '{0}{1}' | t: '1', '1' }}");            
            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(MyFunctions));
            var context = new TemplateContext();

            // act
            context.PushGlobal(scriptObject);
            var result = parsedTemplate.Render(context);

            // assert
            Assert.Equal("11", result);
        }

        [Fact]
        public void Parse_TemplateWithCondition_IsCorrect()
        {
            // arrange
            var template = "{{ products.size > 0 }} {{ products['headphones'].id }} {{ product = products['headphones'] }}{{ products[product.id].id }}";
            // act
            var parsedTemplate = Template.ParseLiquid(template);
            // assert
            Assert.False(parsedTemplate.HasErrors);
        }

        [Fact]
        public void Render_TemplateWithCondition_IsCorrect()
        {
            // arrange
            var parsedTemplate = Template.ParseLiquid("{{ products.size > 0 }} {{ products['headphones'].id }} {{ product = products['headphones'] }}{{ products[product.id].id }}");            
            var scriptObject = new ScriptObject();
            scriptObject.Import(new TestContext { Products = new MutablePagedList<Product>(new List<Product> { new Product { Id = "headphones" } }) });
            var context = new TemplateContext();

            // act
            context.PushGlobal(scriptObject);
            var result = parsedTemplate.Render(context);

            // assert
            Assert.Equal("true headphones headphones", result);
        }


        [Fact]
        public void Parse_TemplateFromFile_IsCorrect()
        {
            // arrange
            var template = File.ReadAllText("./Scriban/test.liquid");
            // act            
            var parsedTemplate = Template.ParseLiquid(template);
            // assert
            Assert.False(parsedTemplate.HasErrors);
        }
    }


    public class TestContext
    {
        public IMutablePagedList<Product> Products { get; set; }
    }
    public class Tag
    {
        public string GroupType { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }

    }
    public class MyFunctions
    {
        public static string T(object input, params object[] variables)
        {
            return string.Format(input.ToString(), variables);
        }

    }

}
