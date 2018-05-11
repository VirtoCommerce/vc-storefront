using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VirtoCommerce.LiquidThemeEngine
{
    public interface ILiquidThemeEngine 
    {
        IEnumerable<string> DiscoveryPaths { get; }
        string ResolveTemplatePath(string templateName);
        string RenderTemplateByName(string templateName, Dictionary<string, object> parameters);
        string RenderTemplate(string templateContent, Dictionary<string, object> parameters);
        IDictionary GetSettings(string defaultValue = null);
        JObject ReadLocalization();
        Stream GetAssetStream(string fileName);
        string GetAssetHash(string fileName);
        string GetAssetAbsoluteUrl(string assetName);
    }
}
