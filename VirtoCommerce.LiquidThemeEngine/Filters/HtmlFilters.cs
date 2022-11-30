namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/html-filters
    /// </summary>
    public static partial class HtmlFilters
    {
        /// <summary>
        /// Generates a script tag.
        /// {{ 'shop.js' | asset_url | script_tag }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ScriptTag(string input)
        {
            return string.Format("<script src=\"{0}\"></script>", input);
        }

        /// <summary>
        /// Generates async script tag.
        /// {{ 'shop.js' | asset_url | script_tag_async }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ScriptTagAsync(string input)
        {
            return string.Format("<script async src=\"{0}\"></script>", input);
        }

        /// <summary>
        /// Generates defer script tag.
        /// {{ 'shop.js' | asset_url | script_tag_defer }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ScriptTagDefer(string input)
        {
            return string.Format("<script defer src=\"{0}\"></script>", input);
        }

        /// <summary>
        /// Generates module script tag.
        /// {{ 'shop.js' | asset_url | script_tag_module }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ScriptTagModule(string input)
        {
            return string.Format("<script type=\"module\" crossorigin src=\"{0}\"></script>", input);
        }

        /// <summary>
        /// Generates a stylesheet tag.
        /// {{ 'shop.css' | asset_url | stylesheet_tag }}
        /// </summary>
        /// <param name="input"></param>
        /// <param name="media"></param>
        /// <returns></returns>
        public static string StylesheetTag(string input, string media = "all")
        {
            return string.Format("<link rel=\"stylesheet\" href=\"{0}\" media=\"{1}\">", input, media);
        }
    }
}
