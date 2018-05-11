using System.Collections.Generic;
using VirtoCommerce.Storefront.Model.Common;
using Xunit;

namespace VirtoCommerce.Storefront.Tests
{
    public class ValueObjectTests
    {
        public class ComplexObject : ValueObject
        {
            public List<string> ListProperty { get; set; }
            public string SimpleProperty { get; set; }         
        }
      
        [Fact]
        public void SameObjectHashCodeAreSame()
        {
            var value1 = new ComplexObject
            {
                SimpleProperty = "A",
                ListProperty = new List<string> { "A", "B", "C" }
            };
            var value2 = new ComplexObject
            {
                SimpleProperty = "A",
                ListProperty = new List<string> { "A", "B", "C" }
            };
            var code1 = value1.GetHashCode();
            var code2 = value2.GetHashCode();
            Assert.Equal(code1, code2);
            Assert.Equal(value1, value2);
        }

    }
}
