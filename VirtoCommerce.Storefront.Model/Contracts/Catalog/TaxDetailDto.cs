namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class TaxDetailDto
    {
        public decimal? Amount { get; set; }

        public string Name { get; set; }

        public decimal? Rate { get; set; }
    }
}
