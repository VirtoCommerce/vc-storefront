using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class DynamicPropertyObjectValueDto
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectId")]
        public string ObjectId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "valueId")]
        public string ValueId { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Undefined', 'ShortText',
        /// 'LongText', 'Integer', 'Decimal', 'DateTime', 'Boolean', 'Html',
        /// 'Image'
        /// </summary>
        [JsonProperty(PropertyName = "valueType")]
        public string ValueType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "propertyId")]
        public string PropertyId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "propertyName")]
        public string PropertyName { get; set; }

    }
}
