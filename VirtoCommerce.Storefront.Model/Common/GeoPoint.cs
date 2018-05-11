using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace VirtoCommerce.Storefront.Model.Common
{
    public class GeoPoint : ValueObject
    {
        public static readonly Regex Regexp = new Regex(@"^([-+]?(?:[1-8]?\d(?:\.\d+)?|90(?:\.0+)?)),\s*([-+]?(?:180(?:\.0+)?|(?:(?:1[0-7]\d)|(?:[1-9]?\d))(?:\.\d+)?))$", RegexOptions.Compiled);
        public GeoPoint()
            : this(0, 0)
        {
        }

        public GeoPoint(double latitude, double longitude)
        {
            if (!IsValidLatitude(latitude))
            {
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Invalid latitude '{0}'. Valid values are between -90 and 90", latitude));
            }
            if (!IsValidLongitude(longitude))
            {
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Invalid longitude '{0}'. Valid values are between -180 and 180", longitude));
            }
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        /// <summary>
		/// True if <paramref name="latitude"/> is a valid latitude. Otherwise false.
		/// </summary>
		/// <param name="latitude"></param>
		/// <returns></returns>
		public static bool IsValidLatitude(double latitude)
        {
            return latitude >= -90 && latitude <= 90;
        }

        /// <summary>
        /// True if <paramref name="longitude"/> is a valid longitude. Otherwise false.
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static bool IsValidLongitude(double longitude)
        {
            return longitude >= -180 && longitude <= 180;
        }

        public override string ToString()
        {
            return Latitude.ToString("#0.0#######", CultureInfo.InvariantCulture) + "," + Longitude.ToString("#0.0#######", CultureInfo.InvariantCulture);
        }

        public static GeoPoint Parse(string value)
        {
            var result = TryParse(value);
            if (result == null)
            {
                throw new ArgumentException("", nameof(value));
            }
            return result;
        }

        public static GeoPoint TryParse(string value)
        {
            GeoPoint result = null;
            var match = Regexp.Match(value);

            if (match.Success && match.Groups.Count == 3)
            {
                result = new GeoPoint
                {
                    Latitude = Math.Round(double.Parse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture), 7),
                    Longitude = Math.Round(double.Parse(match.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture), 7)
                };
            }
            return result;

        }
    }
}
