using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.JsonConverters
{
    /// <summary>
    /// Converter used for API back compatibility of User type. Previous version  contained user and contact data in one type CustomerInfo.
    /// </summary>
    public class UserBackwardCompatibilityJsonConverter : JsonConverter
    {
        private readonly JsonSerializerSettings _jsonSettings;
        private static Type[] _knowTypes = new[] { typeof(User), typeof(UserRegistration)};

        public UserBackwardCompatibilityJsonConverter(JsonSerializerSettings jsonSettings)
        {
            //Making the clone of JsonSerializerSettings, unfortunately cannot find other way to do this
            _jsonSettings = new JsonSerializerSettings
            {
                Context = jsonSettings.Context,
                Culture = jsonSettings.Culture,
                ContractResolver = jsonSettings.ContractResolver,
                ConstructorHandling = jsonSettings.ConstructorHandling,
                CheckAdditionalContent = jsonSettings.CheckAdditionalContent,
                DateFormatHandling = jsonSettings.DateFormatHandling,
                DateFormatString = jsonSettings.DateFormatString,
                DateParseHandling = jsonSettings.DateParseHandling,
                DateTimeZoneHandling = jsonSettings.DateTimeZoneHandling,
                DefaultValueHandling = jsonSettings.DefaultValueHandling,
                EqualityComparer = jsonSettings.EqualityComparer,
                FloatFormatHandling = jsonSettings.FloatFormatHandling,
                Formatting = jsonSettings.Formatting,
                FloatParseHandling = jsonSettings.FloatParseHandling,
                MaxDepth = jsonSettings.MaxDepth,
                MetadataPropertyHandling = jsonSettings.MetadataPropertyHandling,
                MissingMemberHandling = jsonSettings.MissingMemberHandling,
                NullValueHandling = jsonSettings.NullValueHandling,
                ObjectCreationHandling = jsonSettings.ObjectCreationHandling,
                PreserveReferencesHandling = jsonSettings.PreserveReferencesHandling,
                ReferenceLoopHandling = jsonSettings.ReferenceLoopHandling,
                StringEscapeHandling = jsonSettings.StringEscapeHandling,
                TraceWriter = jsonSettings.TraceWriter,
                TypeNameHandling = jsonSettings.TypeNameHandling,
                SerializationBinder = jsonSettings.SerializationBinder,
                TypeNameAssemblyFormatHandling = jsonSettings.TypeNameAssemblyFormatHandling,
                // exclude  BackCompatibilityJsonConverter from  Converters  to prevent infinite loops when serializing
                Converters = null
            };

        }
        public override bool CanWrite { get { return true; } }
        public override bool CanRead { get { return true; } }

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //Use serializer with your setting do not containing this converter to prevent infinite recursion calls.
            serializer = JsonSerializer.Create(_jsonSettings);
            var result = serializer.Deserialize(reader, objectType);

            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var user = value as User;
            //Use serializer with your setting do not containing this converter to prevent infinite recursion calls.
            serializer = JsonSerializer.Create(_jsonSettings);
            var result = JObject.FromObject(user, serializer);
            var contact = user?.Contact;

            if (contact != null)
            {
                var contactJson = JObject.FromObject(contact, serializer);
                result.Merge(contactJson);
                var restoreUserIdJson = JObject.FromObject(new { user.Id } , serializer);
                result.Merge(restoreUserIdJson);
            }
            result.WriteTo(writer);
        }
    }
}
