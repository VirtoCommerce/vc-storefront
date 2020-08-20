using System.Linq;
using VirtoCommerce.Storefront.Model.Catalog;
using Xunit;

namespace VirtoCommerce.Storefront.Tests
{
    public class TermRangeConverterTests
    {
        [Theory]
        [InlineData("under-100", "[ TO 100)", "price", "price_usd", "USD")]
        [InlineData("100-200", "[100 TO 200)", "test", "test_eur", "EUR")]
        [InlineData("over-1000", "[1000 TO )", "price", "price_test", "Test")]
        public void ConvertTerm_Succeed(string termValue, string expectedTermValue, string termName, string expectedTermName, string currencyCode)
        {
            // Arrange
            var term = new Term
            {
                Value = termValue,
                Name = termName,
            };

            // Act
            term.ConvertTerm(currencyCode);

            // Assert
            Assert.Equal(expectedTermName, term.Name);
            Assert.Equal(expectedTermValue, term.Value);
        }

        [Theory]
        [InlineData("price_usd", "[ TO 100)", "price_usd:[ TO 100)")]
        [InlineData("Display size", "5.2", @"\""Display size\"":5.2")]
        [InlineData("Color", "Gold Platinum", @"Color:\""Gold Platinum\""")]
        public void EscapingTerms_Succeed(string termName, string termValue, string expectedResult)
        {
            // Arrange
            var term = new Term
            {
                Value = termValue,
                Name = termName,
            };

            // Act
            var result = new[] {term}.ToStrings(true).FirstOrDefault();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void MultiTermGroupingAndEscaping_Succeed()
        {
            // Arrange
            var terms = new[]
            {
                new Term
                {
                    Name = "color",
                    Value = "gold",
                },
                new Term
                {
                    Name = "color",
                    Value = "gold blue",
                },
            };

            // Act
            var result = terms.ToStrings(true);

            // Assert
            Assert.Equal(@"color:\""gold blue\"",gold", result.Single());
        }
    }
}
