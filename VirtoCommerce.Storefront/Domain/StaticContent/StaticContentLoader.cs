using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model.StaticContent;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.Storefront.Domain
{
    public class StaticContentLoader : IStaticContentLoader
    {
        private static readonly Regex _headerRegExp = new Regex(@"(?s:^---(.*?)---)");

        public virtual string PrepareContent(string content)
        {
            return RemoveYamlHeader(content);
        }

        public virtual void ReadMetaData(string content, IDictionary<string, IEnumerable<string>> metadata)
        {
            ReadYamlHeader(content, metadata);
        }

        private static string RemoveYamlHeader(string text)
        {
            var retVal = text;
            var headerMatches = _headerRegExp.Matches(text);
            if (headerMatches.Count > 0)
            {
                retVal = text.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }
            return retVal;
        }

        private static void ReadYamlHeader(string text, IDictionary<string, IEnumerable<string>> metadata)
        {
            var headerMatches = _headerRegExp.Matches(text);
            if (headerMatches.Count == 0)
                return;

            var input = new StringReader(headerMatches[0].Groups[1].Value);
            var yaml = new YamlStream();

            yaml.Load(input);

            if (yaml.Documents.Count > 0)
            {
                var root = yaml.Documents[0].RootNode;
                var collection = root as YamlMappingNode;
                if (collection != null)
                {
                    foreach (var entry in collection.Children)
                    {
                        var node = entry.Key as YamlScalarNode;
                        if (node != null)
                        {
                            metadata.Add(node.Value, GetYamlNodeValues(entry.Value));
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> GetYamlNodeValues(YamlNode value)
        {
            var retVal = new List<string>();
            var list = value as YamlSequenceNode;

            if (list != null)
            {
                retVal.AddRange(list.Children.OfType<YamlScalarNode>().Select(node => node.Value));
            }
            else
            {
                retVal.Add(value.ToString());
            }

            return retVal;
        }
    }
}
