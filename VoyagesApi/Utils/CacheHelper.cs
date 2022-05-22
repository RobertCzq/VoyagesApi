using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyagesApi.Models;

namespace VoyagesApi.Utils
{
    public static class CacheHelper
    {
        public static void SetUpCache(string cacheKey, IMemoryCache cache, IEnumerable<Voyage> voyageList)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal)
                    .SetSize(1024);
            cache.Set(cacheKey, voyageList, cacheEntryOptions);
        }
    }
}
