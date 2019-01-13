using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.LiquidThemeEngine
{
    public interface ILiquidThemeEngine
    {
        IEnumerable<string> DiscoveryPaths { get; }
        string ResolveTemplatePath(string templateName);
        string RenderTemplateByName(string templateName, object context);
        string RenderTemplate(string templateContent, string templatePath, object context);
        IDictionary<string, object> GetSettings(string defaultValue = null);
        JObject ReadLocalization();
        Stream GetAssetStream(string fileName);
        string GetAssetHash(string fileName);
        string GetAssetAbsoluteUrl(string assetName);
    }
}
