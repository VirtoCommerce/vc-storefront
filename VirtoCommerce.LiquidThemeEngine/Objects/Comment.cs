using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class Comment : Entity
    {
        #region Public Properties
        public string Author { get; set; }

        public string Content { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string Url { get; set; }
        #endregion
    }
}
