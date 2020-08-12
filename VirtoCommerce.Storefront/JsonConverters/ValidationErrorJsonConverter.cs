using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model.Cart.ValidationErrors;
using VirtoCommerce.Storefront.Model.Contracts;

namespace VirtoCommerce.Storefront.JsonConverters
{
    public class ValidationErrorJsonConverter : JsonConverter<ValidationError>
    {
        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override ValidationError ReadJson(JsonReader reader, Type objectType, [AllowNull] ValidationError existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var errorCode = obj["errorCode"].Value<string>();
            var errorObjectId = obj["objectId"].Value<string>();
            var errorObjectType = obj["objectType"].Value<string>();
            var errorMessage = obj["errorMessage"].Value<string>();
            if (errorCode == null)
            {
                throw new NotSupportedException("ErrorCode should be filled for ValidationError instance");
            }

            return new ValidationErrorDto { ErrorCode = errorCode, ErrorMessage = errorMessage, ObjectId = errorObjectId, ObjectType = errorObjectType};
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] ValidationError value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
