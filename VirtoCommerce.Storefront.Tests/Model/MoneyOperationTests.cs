using VirtoCommerce.Storefront.Model.Common;
using Xunit;
using StorefrontLanguage = VirtoCommerce.Storefront.Model.Language;

namespace VirtoCommerce.Storefront.Tests.Model
{
    [Trait("Category", "CI")]
    public class MoneyOperationTests
    {
        private static readonly StorefrontLanguage TestLanguage = new StorefrontLanguage("en-US");
        private static readonly Currency TestCurrency = new Currency(TestLanguage, "TPT", "Test points", "T", 1.0m);

        [Theory]
        [InlineData(0.0, 0.0, 0.0, "T0.00", "T0", "0")]
        [InlineData(0.99, 0.99, 0.99, "T0.99", "T1", "1")]
        [InlineData(0.999, 1.00, 0.99, "T1.00", "T1", "1")]
        [InlineData(1.445, 1.45, 1.44, "T1.45", "T1", "1")]
        [InlineData(1.455, 1.46, 1.45, "T1.46", "T1", "1")]
        //[InlineData( -1.445,  -1.45,  -1.44, "(T1.45)", "(T1)", "(1)")]
        //[InlineData( -1.455,  -1.46,  -1.45, "(T1.46)", "(T1)", "(1)")]
        [InlineData(12.34, 12.34, 12.34, "T12.34", "T12", "12")]
        [InlineData(12.345, 12.35, 12.34, "T12.35", "T12", "12")]
        [InlineData(123.456, 123.46, 123.45, "T123.46", "T123", "123")]
        [InlineData(123.5, 123.5, 123.5, "T123.50", "T124", "124")]
        public void TestGettingMoneyProperties(decimal internalAmount, decimal expectedAmount,
            decimal expectedTruncatedAmount, string expectedFormattedAmount, string expectedFormattedAmountWithoutPoint,
            string expectedFormattedAmountWithoutPointAndCurrency)
        {
            // Arrange
            var money = new Money(internalAmount, TestCurrency);

            // Act & Assert
            Assert.Equal(internalAmount, money.InternalAmount);
            Assert.Equal(expectedAmount, money.Amount);
            Assert.Equal(expectedTruncatedAmount, money.TruncatedAmount);
            Assert.Equal(expectedFormattedAmount, money.FormattedAmount);
            Assert.Equal(expectedFormattedAmountWithoutPoint, money.FormattedAmountWithoutPoint);
            Assert.Equal(expectedFormattedAmountWithoutPointAndCurrency, money.FormattedAmountWithoutPointAndCurrency);
        }
    }
}
