using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Order
{
    public class ShipmentPackageDto
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "barCode")]
        public string BarCode { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "packageType")]
        public string PackageType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "items")]
        public IList<OrderShipmentItemDto> Items { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "weightUnit")]
        public string WeightUnit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "weight")]
        public double? Weight { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "measureUnit")]
        public string MeasureUnit { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "height")]
        public double? Height { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "length")]
        public double? Length { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public double? Width { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "createdDate")]
        //public System.DateTime? CreatedDate { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "modifiedDate")]
        //public System.DateTime? ModifiedDate { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "createdBy")]
        //public string CreatedBy { get; set; }

        ///// <summary>
        ///// </summary>
        //[JsonProperty(PropertyName = "modifiedBy")]
        //public string ModifiedBy { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
