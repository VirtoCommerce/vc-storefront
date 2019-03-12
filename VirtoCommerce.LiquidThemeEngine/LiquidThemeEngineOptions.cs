using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.LiquidThemeEngine
{
    public class LiquidThemeEngineOptions
    {
        public string DefaultLayout { get; set; } = "theme";
        public IList<string> TemplatesDiscoveryFolders { get; set; } = new List<string>() { "templates", "snippets", "layout", "assets" };
        public string ThemesAssetsRelativeUrl { get; set; } = "~/themes/assets";
        public bool RethrowLiquidRenderErrors { get; set; } = false;
        /// <summary>
        /// The name of the base theme that will be used to discover the theme resources not found by the path of theme for current store.
        /// This parameter can be used for theme inheritance logic.
        /// Example values: default_theme -> wwwroot/cms-content/default_theme
        /// </summary>
        public string BaseThemeName { get; set; }
    }
}
