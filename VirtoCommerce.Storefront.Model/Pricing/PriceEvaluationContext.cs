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
    }
}
