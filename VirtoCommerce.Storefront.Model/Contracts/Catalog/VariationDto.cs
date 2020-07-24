using System;

namespace VirtoCommerce.Storefront.Model.Contracts.Catalog
{
    public class VariationDto
    {
        public VariationDto()
        {
            Assets = Array.Empty<AssetDto>();
            Images = Array.Empty<ImageDto>();
            Prices = Array.Empty<PriceDto>();
            Properties = Array.Empty<PropertyDto>();
        }

        public string Id { get; set; }

        public string Code { get; set; }

        public AssetDto[] Assets { get; set; }

        public AvailabilityDataDto AvailabilityData { get; set; }

        public ImageDto[] Images { get; set; }

        public PriceDto[] Prices { get; set; }

        public PropertyDto[] Properties { get; set; }
    }
}
