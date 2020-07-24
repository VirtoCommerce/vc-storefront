using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class PriceDto
    {
        public PriceDto()
        {
            Discounts = Array.Empty<CatalogDiscountDto>();
            TierPrices = Array.Empty<TierPriceDto>();
        }

        public string Currency { get; set; }

        public CatalogDiscountDto[] Discounts { get; set; }

        public MoneyDto List { get; set; }

        public MoneyDto ListWithTax { get; set; }

        public int? MinQuantity { get; set; }

        public string PricelistId { get; set; }

        public MoneyDto Sale { get; set; }

        public MoneyDto SaleWithTax { get; set; }

        public TierPriceDto[] TierPrices { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}
