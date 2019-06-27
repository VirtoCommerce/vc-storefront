using System;
using System.Globalization;

namespace VirtoCommerce.LiquidThemeEngine.Filters
{
    public partial class MathFilters
    {
        public static object Round(object input, int digits = 0)
        {
            if (input != null)
            {
                input = Math.Round(Convert.ToDouble(input, CultureInfo.InvariantCulture), digits);
            }
            return input;
        }

        public static object Ceil(object input)
        {
            if (input != null)
            {
                input = Math.Ceiling(Convert.ToDouble(input, CultureInfo.InvariantCulture));
            }
            return input;
        }

        public static object Floor(object input)
        {
            if (input != null)
            {
                input = Math.Floor(Convert.ToDouble(input, CultureInfo.InvariantCulture));
            }
            return input;
        }

        public static object Abs(object input)
        {
            if (input != null)
            {
                input = Math.Abs(Convert.ToDouble(input, CultureInfo.InvariantCulture));
            }
            return input;
        }

    }
}
