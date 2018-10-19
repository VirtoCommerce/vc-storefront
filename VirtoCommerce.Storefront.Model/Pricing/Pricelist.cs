using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Pricing
{
    public partial class Pricelist
    {
        public Pricelist(Currency currency)
        {
            Currency = currency;
        }
        public string Id { get; set; }
        public Currency Currency { get; set; }
    }
}
