using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Caching;
using VirtoCommerce.Storefront.Model.Common.Caching;
using VirtoCommerce.Storefront.Model.Common.Exceptions;

namespace VirtoCommerce.Storefront.Domain
{
    public class FileSystemCountriesService : ICountriesService
    {
        private readonly FileSystemCountriesOptions _options;
        private readonly IStorefrontMemoryCache _memoryCache;
        public FileSystemCountriesService(IStorefrontMemoryCache cacheManager, IOptions<FileSystemCountriesOptions> options)
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
                var result = new List<Country>();

                if (_options != null)
                {
                    var countriesJson = await File.ReadAllTextAsync(_options.FilePath);
                    var countriesDict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(countriesJson);

                    result = countriesDict
                        .Select(ParseCountry)
                        .Where(c => !string.IsNullOrEmpty(c.Code3))
                        .ToList();
                }
                return result;
            });
        }
        #endregion

        protected static Country ParseCountry(KeyValuePair<string, JObject> pair)
        {
            var country = new Country
            {
                Name = pair.Key,
                Code2 = pair.Value["Code2"]?.ToString(),
                Code3 = pair.Value["Code3"]?.ToString(),
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
