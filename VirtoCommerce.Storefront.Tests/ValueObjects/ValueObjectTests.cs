using System;
using System.Collections.Generic;
using System.Text;
using VirtoCommerce.Storefront.Model.Common;
using Xunit;

namespace VirtoCommerce.Storefront.Tests.ValueObjects
{
    public class ValueObjectTests
    {
        public class ComplexObject : ValueObject
        {
            public List<string> ListProperty { get; set; }
            public string SimpleProperty { get; set; }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return SimpleProperty;
                foreach(var val in ListProperty)
                {
                    yield return val;
                }
            }
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
        }

    }
}
