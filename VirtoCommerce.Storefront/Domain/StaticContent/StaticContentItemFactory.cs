using System.IO;
using System.Text.RegularExpressions;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.StaticContent;

namespace VirtoCommerce.Storefront.Domain
{
    public class StaticContentItemFactory : IStaticContentItemFactory
    {
        private static readonly Regex _blogMatchRegex = new Regex(@"blogs/(?<blog>[^\/]*)\/([^\/]*)\.[^\.]+$", RegexOptions.Compiled);

        public ContentItem GetItemFromPath(string path)
        {
            ContentItem retVal = null;
            if (!string.IsNullOrEmpty(path))
            {
                //Blog
                var blogMatch = _blogMatchRegex.Match(path);
                if (blogMatch.Success)
                {
                    var blogName = blogMatch.Groups["blog"].Value;
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    if (fileName.EqualsInvariant(blogName) || fileName.EqualsInvariant("default"))
                    {
                        retVal = new Blog()
                        {
                            Name = blogName,
                        };
                    }
                    else
                    {
                        retVal = new BlogArticle()
                        {
                            BlogName = blogName
                        };
                    }
                }
                else
                {
                    retVal = new ContentPage();
                }
            }
                
            return retVal;
        }
    }
}