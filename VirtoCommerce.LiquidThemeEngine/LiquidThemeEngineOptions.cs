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
        /// The path to the base theme that will be used to discover the theme resources not found by the path of theme for current store.
        /// This parameter can be used for theme inheritance logic.
        /// Example values:
        ///   Electronics/default -> wwwroot/cms-content/Themes/Electronics/default
        ///   default-> wwwroot/cms-content/Themes/default
        /// </summary>
        public string BaseThemePath { get; set; }

        /// <summary>
        /// Original description:
        /// The name of the base theme that will be used to discover the theme resources not found by the path of theme for current store.
        /// This parameter can be used for theme inheritance logic.
        /// Example values: default_theme -> wwwroot/cms-content/default_theme
        ///
        /// How it actually worked:
        /// Storefront used this parameter as a store name, i.e. Electronics -> wwwroot/cms-content/Themes/Electronics/default
        /// </summary>
        [Obsolete("Obsolete. Use BaseThemePath instead.")]
        public string BaseThemeName { get; set; }

        /// <summary>
        /// Set to true if you want to merge current theme settings with base theme settings instead of placement
        /// </summary>
        public bool MergeBaseSettings { get; set; }
    }
}
