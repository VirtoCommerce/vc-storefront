using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Notification : ValueObject
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }
    }
}
