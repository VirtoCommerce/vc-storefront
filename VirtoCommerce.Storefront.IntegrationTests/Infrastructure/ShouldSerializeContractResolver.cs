using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VirtoCommerce.Storefront.Model;

namespace VirtoCommerce.Storefront.IntegrationTests.Infrastructure
{
    public class CustomContractResolver : CamelCasePropertyNamesContractResolver
    {
        public new static readonly CustomContractResolver Instance = new CustomContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType == typeof(TierPrice))
            {
                property.ShouldSerialize = instance => false;
            }

            return property;
        }
    }
}
