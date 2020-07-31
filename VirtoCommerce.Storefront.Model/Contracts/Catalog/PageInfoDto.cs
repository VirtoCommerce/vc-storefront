namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class PageInfoDto
    {
        public string EndCursor { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public string StartCursor { get; set; }
    }
}
