using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Domain
{
    public class FileSystemCountriesService: ICountriesService
    {
        private readonly FileSystemCountriesOptions _options;
        private readonly IMemoryCache _memoryCache;
        public FileSystemCountriesService(IMemoryCache cacheManager, IOptions<FileSystemCountriesOptions> options)
        {
            _options = options.Value;
            _memoryCache = cacheManager;
        }
        #region IKnowCountriesProvider members
        public async Task<IList<Country>> GetCountriesAsync()
        {
            if (_options == null)
            {
                throw new StorefrontException("the path to countries json file not set");
            }

            var cacheKey = CacheKey.With(GetType(), "GetCountries");
            return await _memoryCache.GetOrCreateAsync(cacheKey, async (cacheEntry) =>
            {
                List<Country> result = new List<Country>();

                var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Select(GetRegionInfo)
                    .Where(r => r != null)
                    .ToList();

                if (_options != null)
                {
                    var countriesJson = await File.ReadAllTextAsync(_options.FilePath);
                    var countriesDict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(countriesJson);

                    result = countriesDict
                        .Select(kvp => ParseCountry(kvp, regions))
                        .Where(c => c.Code3 != null)
                        .ToList();
                }
                return result;
            });

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
