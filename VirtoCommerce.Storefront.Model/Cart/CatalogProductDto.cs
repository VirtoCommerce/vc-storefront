namespace VirtoCommerce.Storefront.Model.Cart
{
    public class CatalogProductDto
    {
        public string Description { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public string Code { get; set; }
        public string ImgSrc { get; set; }
        public string OuterId { get; set; }
        //Field<ListGraphType<PropertyType>>("properties", resolve: context => context.Source.CatalogProduct.Properties);

        //    Field<ListGraphType<PriceType>>("prices"
    }
}
