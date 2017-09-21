using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class LiquidThemeEngineOptions
    {
        public LiquidThemeEngineOptions()
        {
            DefaultLayout = "theme";
            ThemesAssetsRelativeUrl = "~/themes/assets";
            TemplatesDiscoveryFolders = new List<string>() { "templates", "snippets", "layout", "assets" };
            RethrowLiquidRenderErrors = false;
        }

        public string DefaultLayout { get; set; }
        public IList<string> TemplatesDiscoveryFolders { get; set; }
        public string ThemesAssetsRelativeUrl { get; set; }
        public bool RethrowLiquidRenderErrors { get; set; }
    }
}
