using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;
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
        public void Pipe_Arguments_Mismatch_Errors2()
        {
            var parsedTemplate = Template.ParseLiquid("{{ 'some resource key' | t }}");
            Assert.False(parsedTemplate.HasErrors);

            var scriptObject = new ScriptObject();
            scriptObject.Import(typeof(MyFunctions));
            var context = new TemplateContext();
            context.PushGlobal(scriptObject);

            parsedTemplate.Render(context);
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
        public void ParsingError()
        {
            var parsedTemplate = Template.ParseLiquid(File.ReadAllText(@"C:\Projects\VirtoCommerce\vc-storefront-core\VirtoCommerce.Storefront.Tests\Scriban\test.liquid"));
            Assert.False(parsedTemplate.HasErrors);
        }
    }

    public class MyFunctions
    {
        public static string T(TemplateContext context, object input, params object[] variables)
        {
            return input.ToString();
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
