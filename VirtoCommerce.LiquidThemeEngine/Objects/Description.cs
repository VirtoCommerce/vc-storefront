using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Description : ValueObject, IDictionaryKey
    {
        public string Type { get; set; }

        public string Content { get; set; }

        #region IDictionaryKey
        public string Key => Type;
        #endregion
    }
}
