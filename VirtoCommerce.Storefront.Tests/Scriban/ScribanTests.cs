using System.Collections.Generic;
using System.IO;
using Scriban;
using Scriban.Runtime;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Stores;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.Scriban
{
    public class ScribanTests
    {
        /// <summary>
        /// https://github.com/lunet-io/scriban/issues/102
        /// </summary>
        [Fact]
        public void LiquidRawBlock_ParsingError()
        {
            var parsedTemplate = Template.ParseLiquid("{% raw %}{% endraw %}{{ some_var }}");
            Assert.False(parsedTemplate.HasErrors);
        }

        [Fact]
        public void Capture_With_Elsif()
        {
            var template = "{% capture tag_label_template %}tags.{{ tag.group_type }}.{% if tag.lower and tag.upper %}between{% elsif tag.lower %}greater{% elsif tag.upper %}less{% endif %}{% endcapture %} {{ tag_label_template }}";
            var parsedTemplate = Template.ParseLiquid(template);
            Assert.False(parsedTemplate.HasErrors);

            var scriptObject = new ScriptObject();
            scriptObject.SetValue("tag", new Tag { GroupType = "pricerange", Lower = "11$" }, true);
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            var result = parsedTemplate.Render(context);
            Assert.Equal("tags.pricerange.greater", result);
        }
        [Fact]
        public void Call_Pipe_Function_With_Named_Argument_Throw_Exception()
        {
            var parsedTemplate = Template.ParseLiquid("{{ math.plus value: 1 with: 2 }}");
            Assert.False(parsedTemplate.HasErrors);

            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(MyFunctions));
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            var result = parsedTemplate.Render(context);
            Assert.Equal("12", result);
        }

        [Fact]
        public void Function_With_Context_And_Params_Throw_Overflow_Exception()
        {
            var parsedTemplate = Template.ParseLiquid("{{ '{0}{1}' | t: '1', '1'  }}");
            Assert.False(parsedTemplate.HasErrors);

            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(MyFunctions));
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            var result = parsedTemplate.Render(context);
            Assert.Equal("11", result);
        }


        [Fact]
        public void Pipe_Arguments_Mismatch_Errors()
        {
            var parsedTemplate = Template.ParseLiquid("{{ 22.00 | A | B | string.upcase }}");
            Assert.False(parsedTemplate.HasErrors);

            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(MyFunctions));
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            parsedTemplate.Render(context);
        }

        [Fact]
        public void IndexAccess_For_List()
        {
            var parsedTemplate = Template.ParseLiquid("{{ products.size > 0 }} {{ products['headphones'].id }} {{ product = products['headphones'] }} {{ products[product.id].id }}");
            Assert.False(parsedTemplate.HasErrors);

            var scriptObject = new ScriptObject();
            scriptObject.Import(new TestContext { Products = new MutablePagedList<Store>(new List<Store> { new Store { Id = "headphones" } }) });
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            var result = parsedTemplate.Render(context);
            Assert.Equal("true headphones headphones", result);
        }

        [Fact]
        public void ParsingError()
        {
            var parsedTemplate = Template.ParseLiquid(File.ReadAllText(@"C:\Projects\VirtoCommerce\vc-storefront-core\VirtoCommerce.Storefront.Tests\Scriban\test.liquid"));
            Assert.False(parsedTemplate.HasErrors);
        }
    }


    public class TestContext
    {
        public IMutablePagedList<Store> Products { get; set; }
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

        public static string A(TemplateContext context, object input, string currencyCode = null)
        {
            return input.ToString() + "A";
        }
        public static string B(object input)
        {
            return input.ToString() + "B";
        }

    }

}
