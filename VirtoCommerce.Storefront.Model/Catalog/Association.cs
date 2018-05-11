namespace VirtoCommerce.Storefront.Model.Catalog
{
    public  abstract class Association
    {
        /// <summary>
        /// Association type Related, Associations, Up-Sales etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Association priority 0 min 
        /// </summary>
        public int Priority { get; set; }

        public Image Image { get; set; }

        public int? Quantity { get; set; }
    }
}
