using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig;
using YamlDotNet.RepresentationModel;

namespace VirtoCommerce.Storefront.Domain
{
    public class MarkdownReader : ContentItemReader
    {
        private static readonly Regex _headerRegExp = new Regex(@"(?s:^---(.*?)---)");
        private static string _excerptToken = "<!--excerpt-->";

        public MarkdownReader(string path, string content) : base(path, content) { }

        public override string ReadContent()
        {
            var result = Content;
            var headerMatches = _headerRegExp.Matches(result);
            if (headerMatches.Count > 0)
            {
                result = result.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }
            result = result.Replace(_excerptToken, string.Empty);
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(result, pipeline);
        }

        public override Dictionary<string, IEnumerable<string>> ReadMetadata()
        {
            var result = new Dictionary<string, IEnumerable<string>>();
            var contentWithoutYaml = ReadYamlMetadata(result);
            ReadExcerpt(result, contentWithoutYaml);
            AddPropertiesFromFilename(result);
            return result;
        }

        private void ReadExcerpt(Dictionary<string, IEnumerable<string>> metadata, string contentWithoutYaml)
        {

            if (contentWithoutYaml.Contains(_excerptToken))
            {
                var parts = contentWithoutYaml.Split(new[] { _excerptToken }, StringSplitOptions.None);
                if (parts.Length > 1)
                {
                    metadata.Add("excerpt", new[] { parts[0] });
                }
            }
        }

        private string ReadYamlMetadata(Dictionary<string, IEnumerable<string>> metadata)
        {
            var headerMatches = _headerRegExp.Matches(Content);
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
                return Content.Replace(headerMatches[0].Groups[0].Value, "").Trim();
            }
            return Content;
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
