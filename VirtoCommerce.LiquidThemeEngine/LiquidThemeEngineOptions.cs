using System.Collections.Generic;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class LiquidThemeEngineOptions
    {
        public string DefaultLayout { get; set; } = "theme";
        public IList<string> TemplatesDiscoveryFolders { get; set; } = new List<string>() { "templates", "snippets", "layout", "assets" };
        public string ThemesAssetsRelativeUrl { get; set; } = "~/themes/assets";
        public bool RethrowLiquidRenderErrors { get; set; } = false;
    }
}
