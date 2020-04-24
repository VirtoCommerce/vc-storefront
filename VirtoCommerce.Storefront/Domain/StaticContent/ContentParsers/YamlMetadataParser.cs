using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtoCommerce.Storefront.Model.StaticContent;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.Storefront.Domain
{
    public partial class StaticContentItemBuilder
    {
        private class YamlMetadataParser : IContentItemParser
        {
            public bool Suit(ContentItem item)
            {
                return _extensions.Any(item.FileName.EndsWith);
            }

            public void Parse(string path, string content, ContentItem item)
            {
                item.MetaInfo = new Dictionary<string, IEnumerable<string>>();
                var headerMatches = _headerRegExp.Matches(content);
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
}
