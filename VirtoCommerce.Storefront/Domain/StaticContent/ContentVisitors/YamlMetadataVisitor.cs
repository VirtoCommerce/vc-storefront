using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.Storefront.Domain
{
    internal class YamlMetadataVisitor : IContentItemVisitor
    {
        public bool Suit(ContentItem item)
        {
            return StaticContentItemBuilder.extensions.Any(item.FileName.EndsWith);
        }

        public string ReadContent(string path, string content, ContentItem item)
        {
            item.MetaInfo = new Dictionary<string, IEnumerable<string>>();
            var headerMatches = StaticContentItemBuilder.headerRegExp.Matches(content);
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
                                item.MetaInfo.Add(node.Value, GetYamlNodeValues(entry.Value));
                            }
                        }
                    }
                }
                return content.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }
            return content;
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
