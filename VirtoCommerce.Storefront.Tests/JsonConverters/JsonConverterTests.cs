using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Storefront.Model.Security;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.JsonConverters
{
    public class JsonConverterTests
    {    
        [Fact]
        public void SerializeUser()
        {
            var user = new User
            {

            };
            var settings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var serializer = JsonSerializer.Create(settings);
            var result = JObject.FromObject(user, serializer);
            Assert.NotNull(result);
        }

    }
}
