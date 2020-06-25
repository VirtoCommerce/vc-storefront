using System.Collections.Generic;
using Newtonsoft.Json;

namespace VirtoCommerce.Storefront.Model.Cart
{
    public class DynamicObjectPropertyDto
    {
        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectId")]
        public string ObjectId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "values")]
        public IList<DynamicPropertyObjectValueDto> Values { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "objectType")]
        public string ObjectType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isArray")]
        public bool? IsArray { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isDictionary")]
        public bool? IsDictionary { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isMultilingual")]
        public bool? IsMultilingual { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "isRequired")]
        public bool? IsRequired { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "displayOrder")]
        public int? DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets possible values include: 'Undefined', 'ShortText',
        /// 'LongText', 'Integer', 'Decimal', 'DateTime', 'Boolean', 'Html',
        /// 'Image'
        /// </summary>
        [JsonProperty(PropertyName = "valueType")]
        public string ValueType { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "displayNames")]
        public IList<DynamicPropertyName> DisplayNames { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdDate")]
        public System.DateTime? CreatedDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedDate")]
        public System.DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "modifiedBy")]
        public string ModifiedBy { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

    }
}
