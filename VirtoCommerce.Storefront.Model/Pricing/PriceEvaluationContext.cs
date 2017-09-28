using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Storefront.Model.Common;

namespace VirtoCommerce.Storefront.Model.Pricing
{
    public class PriceEvaluationContext : ValueObject
    {       
        public string StoreId { get; set; }
        public string CatalogId { get; set; }
        public IList<string> ProductIds { get; set; } = new List<string>();
        public IList<string> PricelistIds { get; set; } = new List<string>();
        public double? Quantity { get; set; }

        public string CustomerId { get; set; }

        public string OrganizationId { get; set; }

        public System.DateTime? CertainDate { get; set; }

        public string Currency { get; set; }

        public object ContextObject { get; set; }

        public string GeoCity { get; set; }

        public string GeoState { get; set; }

        public string GeoCountry { get; set; }

        public string GeoContinent { get; set; }

        public string GeoZipCode { get; set; }

        public string GeoConnectionType { get; set; }

        public string GeoTimeZone { get; set; }
        
        public string GeoIpRoutingType { get; set; }
        
        public string GeoIspSecondLevel { get; set; }
        
        public string GeoIspTopLevel { get; set; }
        
        public int? ShopperAge { get; set; }
        
        public string ShopperGender { get; set; }
        
        public string Language { get; set; }

        public IList<string> UserGroups { get; set; } = new List<string>();

        public string ShopperSearchedPhraseInStore { get; set; }
        
        public string ShopperSearchedPhraseOnInternet { get; set; }
        
        public string CurrentUrl { get; set; }
        
        public string ReferredUrl { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StoreId;
            yield return CatalogId;
            yield return Quantity;
            yield return CustomerId;
            yield return OrganizationId;
            yield return Currency;
            yield return ContextObject;
            yield return GeoCity;
            yield return GeoState;
            yield return GeoCountry;
            yield return GeoContinent;
            yield return GeoZipCode;
            yield return GeoConnectionType;
            yield return GeoTimeZone;
            yield return GeoIpRoutingType;
            yield return GeoIspSecondLevel;
            yield return GeoIspTopLevel;
            yield return ShopperAge;
            yield return ShopperGender;
            yield return Language;
            yield return ShopperSearchedPhraseInStore;
            yield return ShopperSearchedPhraseOnInternet;
            yield return CurrentUrl;
            yield return ReferredUrl;

            if(!ProductIds.IsNullOrEmpty())
            {               
                foreach(var productId in ProductIds)
                {
                    yield return productId;
                }
            }
            if (!UserGroups.IsNullOrEmpty())
            {
                foreach (var userGroup in UserGroups)
                {
                    yield return userGroup;
                }
            }
            if (!PricelistIds.IsNullOrEmpty())
            {
                foreach (var priceList in PricelistIds)
                {
                    yield return priceList;
                }
            }

        }

    }
}
