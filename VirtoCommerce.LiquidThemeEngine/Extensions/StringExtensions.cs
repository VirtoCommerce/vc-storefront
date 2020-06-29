using System.Globalization;

namespace DotLiquid.ViewEngine.Extensions
{
    public static class StringExtensions
    {
        public static string TrimQuotes(this string input)
        {
            return input.Trim('\'', '\"');
        }

        public static CultureInfo TryGetCultureInfo(this string languageCode)
        {
            try
            {
                return !string.IsNullOrEmpty(languageCode) ? CultureInfo.CreateSpecificCulture(languageCode) : null;
            }
            catch
            {
                return null;
            }
        }

        public static int SafeParseInt(this string input, int defaultValue = default)
        {
            return int.TryParse(input, out var result) ? result : defaultValue;
        }
    }
}
