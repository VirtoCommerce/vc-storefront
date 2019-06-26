using System;
using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Pricing
{
    public partial class PriceEvaluationContext : ValueObject
    {
        public string StoreId { get; set; }
        public string CatalogId { get; set; }
        public DateTime? CertainDate { get; set; }
        public IList<string> ProductIds { get; set; } = new List<string>();
        public IList<string> PricelistIds { get; set; } = new List<string>();
        public double? Quantity { get; set; }

        public string CustomerId { get; set; }

        public string OrganizationId { get; set; }

        public string Currency { get; set; }
        public string GeoCity { get; set; }
        public string GeoCountry { get; set; }
        public string GeoState { get; set; }
        public string GeoZipCode { get; set; }
        public string GeoTimeZone { get; set; }

        public string GeoIpRoutingType { get; set; }

        public string GeoIspSecondLevel { get; set; }

        public string GeoIspTopLevel { get; set; }

        public int? ShopperAge { get; set; }

        public string ShopperGender { get; set; }

        public string Language { get; set; }

        public IList<string> UserGroups { get; set; } = new List<string>();


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CatalogId;
            yield return Currency;
            yield return StoreId;
            yield return OrganizationId;
            yield return Language;
            yield return Quantity;
            yield return CertainDate;

            //Remove user for equality because marketing promotions very rarely depend on concrete customer and exclude  user from  cache key can have significant affect to performance
            //yield return CustomerId;
            yield return string.Join('&', ProductIds ?? Array.Empty<string>());
            yield return string.Join('&', PricelistIds ?? Array.Empty<string>());
            yield return string.Join('&', UserGroups ?? Array.Empty<string>());
        }
    }
}
