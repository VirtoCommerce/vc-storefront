using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace VirtoCommerce.Storefront.Caching
{
    public interface IStorefrontMemoryCache : IMemoryCache
    {
    }
}
