using CacheManager.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using VirtoCommerce.Storefront.Common;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Services
{
    public class JsonCountriesService : ICountriesService
    {
        private readonly string _countriesJsonPath;
        private readonly ICacheManager<object> _cacheManager;
        public JsonCountriesService(ICacheManager<object> cacheManager, string countriesJsonPath)
        {
            _countriesJsonPath = countriesJsonPath;
            _cacheManager = cacheManager;
        }
        #region IKnowCountriesProvider members
        public IEnumerable<Country> GetCountries()
        {
            if (_countriesJsonPath == null)
            {
                throw new StorefrontException("the path to countries json file not set");
            }

            return _cacheManager.Get("GetCountries", StorefrontConstants.CountryCacheRegion, () =>
            {
                Country[] result = null;

                var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Select(GetRegionInfo)
                    .Where(r => r != null)
                    .ToList();

                if (_countriesJsonPath != null)
                {
                    var countriesJson = File.ReadAllText(_countriesJsonPath);
                    var countriesDict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(countriesJson);

                    result = countriesDict
                        .Select(kvp => ParseCountry(kvp, regions))
                        .Where(c => c.Code3 != null)
                        .ToArray();
                }
                return result;

            }, cacheNullValue: false);

        }
        #endregion

        protected static RegionInfo GetRegionInfo(CultureInfo culture)
        {
            RegionInfo result = null;

            try
            {
                result = new RegionInfo(culture.LCID);
            }
            catch
            {
                // ignored
            }

            return result;
        }

        protected static Country ParseCountry(KeyValuePair<string, JObject> pair, List<RegionInfo> regions)
        {
            var region = regions.FirstOrDefault(r => string.Equals(r.EnglishName, pair.Key, StringComparison.OrdinalIgnoreCase));

            var country = new Country
            {
                Name = pair.Key,
                Code2 = region?.TwoLetterISORegionName ?? string.Empty,
                Code3 = region?.ThreeLetterISORegionName ?? string.Empty,
                RegionType = pair.Value["label"]?.ToString()
            };

            var provinceCodes = pair.Value["province_codes"].ToObject<Dictionary<string, string>>();
            if (provinceCodes != null && provinceCodes.Any())
            {
                country.Regions = provinceCodes
                    .Select(kvp => new CountryRegion { Name = kvp.Key, Code = kvp.Value })
                    .ToArray();
            }

            return country;
        }
    }
}
