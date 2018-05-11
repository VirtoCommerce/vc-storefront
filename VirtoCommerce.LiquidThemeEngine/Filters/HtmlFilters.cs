namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    /// <summary>
    /// https://docs.shopify.com/themes/liquid-documentation/filters/html-filters
    /// </summary>
    public class HtmlFilters
    {
        /// <summary>
        /// Generates a script tag.
        /// {{ 'shop.js' | asset_url | script_tag }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ScriptTag(string input)
        {
            return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", input);
        }

        /// <summary>
        /// Generates a stylesheet tag.
        /// {{ 'shop.css' | asset_url | stylesheet_tag }}
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StylesheetTag(string input)
        {
            return string.Format("<link type=\"text/css\" rel=\"stylesheet\" media=\"all\" href=\"{0}\" />", input);
        }

        /// <summary>
        /// Generates an image tag
        /// {{ 'smirking_gnome.gif' | asset_url | img_tag }}
        /// </summary>
        /// <param name="input"></param>
        /// <param name="alt"></param>
        /// <param name="css"></param>
        /// <returns></returns>
        public static string ImgTag(string input, string alt = "", string css = "")
        {
            return input == null ? null : GetImageTag(input, alt, css);
        }


        private static string GetImageTag(string src, string alt, string css)
        {
            return string.Format("<img alt=\"{0}\" src=\"{1}\" class=\"{2}\" />", alt, src, css);
        }
    }
}
