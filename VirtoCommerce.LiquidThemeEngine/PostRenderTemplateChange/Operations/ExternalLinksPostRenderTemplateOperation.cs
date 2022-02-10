using System.Linq;
using System.Text.RegularExpressions;

namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations
{
    public class ExternalLinksPostRenderTemplateOperation : IPostRenderTemplateChangeOperation
    {
        public void Run(ref string renderResult)
        {
            var linksTagsRegex = new Regex(@"<\s*a[^>]*>(.*?)<\s*/\s*a>");
            var hrefAttrRegex = new Regex(@"(?<=\bhref="")[^""]*");
            var matches = linksTagsRegex.Matches(renderResult).Where(m => m.Success).Select(m => m.Value);
            foreach (var match in matches)
            {
                var hrefAttrValue = hrefAttrRegex.Match(match).Value;
                if (hrefAttrValue.Trim().StartsWith("http"))
                {
                    var matchWithRel = match.Replace("<a", "<a rel=\"nofollow\" target=\"_blank\"");
                    renderResult = renderResult.Replace(match, matchWithRel);
                }
            }
        }
    }
}
