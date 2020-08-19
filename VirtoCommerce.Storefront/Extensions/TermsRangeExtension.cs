using System.Linq;
using VirtoCommerce.Storefront.Model.Catalog;

namespace VirtoCommerce.Storefront.Extensions
{
    public static class TermsRangeExtension
    {
        public static void ConvertTerm(this Term term)
        {
            var values = term.Value.Split('-');
            var isLowerLimitInteger = int.TryParse(values.FirstOrDefault(), out var lowerLimit);
            var isUpperLimitInteger = int.TryParse(values.Skip(1).FirstOrDefault(), out var upperLimit);

            term.Value = $"[{(isLowerLimitInteger ? lowerLimit.ToString() : string.Empty)} TO {(isUpperLimitInteger ? upperLimit.ToString() : string.Empty)})";
        }
    }
}
