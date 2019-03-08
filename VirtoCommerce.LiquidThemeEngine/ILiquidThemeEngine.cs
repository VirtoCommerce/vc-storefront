using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VirtoCommerce.LiquidThemeEngine
{
    public interface ILiquidThemeEngine
    {
        IEnumerable<string> DiscoveryPaths { get; }
        string ResolveTemplatePath(string templateName);
        Task<string> RenderTemplateByNameAsync(string templateName, object context);
        Task<string> RenderTemplateAsync(string templateContent, string templatePath, object context);
        IDictionary<string, object> GetSettings(string defaultValue = null);
        JObject ReadLocalization();
        Task<Stream> GetAssetStreamAsync(string fileName);
        string GetAssetHash(string fileName);
        string GetAssetAbsoluteUrl(string assetName);
    }
}
