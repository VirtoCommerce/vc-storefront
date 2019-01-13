using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model
{
    /// <summary>
    /// Represents site navigation menu link object
    /// </summary>
    public class MenuLink : Entity, IAccessibleByIndexKey
    {
        /// <summary>
        /// Gets or sets the title of site navigation menu link
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Returns the type of the link. The possible values are:
        /// collection: if the link points to a collection
        /// product: if the link points to a product page   
        /// </summary>
        public virtual string Type => AssociatedObjectType;
        /// <summary>
        /// Gets or sets the URL of site navigation menu link
        /// </summary>        
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the priority of site navigation menu link
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Each link element can has a associated object like a Product, Category, Promotion etc.
        /// Is a primary key for associated object
        /// </summary>
        public string AssociatedObjectId { get; set; }

        /// <summary>
        /// Associated object type
        /// </summary>
        public string AssociatedObjectType { get; set; }

        public string IndexKey => Id;
    }
}
