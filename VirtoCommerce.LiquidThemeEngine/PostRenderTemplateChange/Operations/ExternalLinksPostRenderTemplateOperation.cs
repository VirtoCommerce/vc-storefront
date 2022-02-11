using System.Linq;
using System.Text.RegularExpressions;

namespace VirtoCommerce.LiquidThemeEngine.PostRenderTemplateChange.Operations
{
    public class ExternalLinksPostRenderTemplateOperation : IPostRenderTemplateChangeOperation
    {
        private readonly Regex _linksTagsRegex = new Regex(@"<\s*a[^>]*>(.*?)<\s*/\s*a>", RegexOptions.Compiled);
        private readonly Regex _hrefAttrRegex = new Regex(@"(?<=\bhref\s*=\s*[""'])[^""']*", RegexOptions.Compiled);

        public string Run(string renderResult)
        {
            var matches = _linksTagsRegex.Matches(renderResult).Where(m => m.Success).Select(m => m.Value);
            foreach (var match in matches)
            {
                var hrefAttrValue = _hrefAttrRegex.Match(match).Value.Trim().ToUpper();
                if (hrefAttrValue.StartsWith("HTTP:") || hrefAttrValue.StartsWith("HTTPS:"))
                {
                    var matchWithRel = match.Replace("<a", "<a rel=\"nofollow\" target=\"_blank\"");
                    renderResult = renderResult.Replace(match, matchWithRel);
                }
            }
            return renderResult;
        }
    }
}
