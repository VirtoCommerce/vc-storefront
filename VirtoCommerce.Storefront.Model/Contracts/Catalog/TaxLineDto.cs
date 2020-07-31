namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class TaxLineDto
    {
        public decimal? Amount { get; set; }

        public string Code { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public string TaxType { get; set; }
    }
}
