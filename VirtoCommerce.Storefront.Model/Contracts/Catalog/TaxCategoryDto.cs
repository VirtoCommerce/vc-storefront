using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class TaxCategoryDto
    {
        public TaxCategoryDto()
        {
            Rates = Array.Empty<TaxRateDto>();
        }

        public TaxRateDto[] Rates { get; set; }
    }
}
