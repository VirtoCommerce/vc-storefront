using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class LiquidThemeEngineSettings
    {
        public LiquidThemeEngineSettings()
        {
            DefaultLayout = "theme";
            TemplatesDiscoveryFolders = new [] { "templates", "snippets", "layout", "assets" };
            RethrowLiquidRenderErrors = false;

        }
        public string DefaultLayout { get; set; }
        public string[] TemplatesDiscoveryFolders { get; set; }
        public string ThemesAssetsRelativeUrl { get; set; }
        public bool RethrowLiquidRenderErrors { get; set; }
    }
}
