using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            var result = text;
            var headerMatches = _headerRegExp.Matches(text);

            if (headerMatches.Count > 0)
            {
                result = text.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }

            return result;
        }

        private static void ReadYamlHeader(string text, IDictionary<string, IEnumerable<string>> metadata)
        {
            var headerMatches = _headerRegExp.Matches(text);

            if (headerMatches.Count > 0)
            {
                var input = new StringReader(headerMatches[0].Groups[1].Value);
                var yaml = new YamlStream();

                yaml.Load(input);

                if (yaml.Documents.Count > 0)
                {
                    var root = yaml.Documents[0].RootNode;
                    if (root is YamlMappingNode collection)
                    {
                        foreach (var entry in collection.Children)
                        {
                            if (entry.Key is YamlScalarNode node)
                            {
                                metadata.Add(node.Value, GetYamlNodeValues(entry.Value));
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> GetYamlNodeValues(YamlNode value)
        {
            var result = new List<string>();

            if (value is YamlSequenceNode list)
            {
                result.AddRange(list.Children.OfType<YamlScalarNode>().Select(node => node.Value));
            }
            else
            {
                result.Add(value.ToString());
            }

            return result;
        }
    }
}
