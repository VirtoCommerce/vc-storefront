using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class TaxRateDto
    {
        public TaxRateDto()
        {
            TaxDetails = Array.Empty<TaxDetailDto>();
        }

        public TaxLineDto Line { get; set; }

        public decimal? PercentRate { get; set; }

        public decimal? Rate { get; set; }

        public TaxDetailDto[] TaxDetails { get; set; }

        public string TaxProviderCode { get; set; }
    }
}
