using DotLiquid;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class ProductProperty : Drop
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// Property value type
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// Property value in current language
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Property hidden
        /// </summary>
        public bool Hidden { get; set; }
    }
}
