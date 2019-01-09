using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.LiquidThemeEngine.Objects
{
    public partial class ArticleUser : ValueObject
    {
        #region Public Properties
        public string AccountOwner { get; set; }

        public string Bio { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Homepage { get; set; }

        public string LastName { get; set; }
        #endregion
    }
}
