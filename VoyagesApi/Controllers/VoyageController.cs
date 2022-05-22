using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyagesApi.Models;

namespace VoyagesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VoyageController : ControllerBase
    {
        private Voyage[] voyageList = new Voyage[]
        {
            new Voyage
            {
                VoyageCode = "451S",
                Price = 121,
                Currency = Currency.USD,
                Timestamp = DateTimeOffset.Now
            },
            new Voyage
            {
                VoyageCode = "451S",
                Price = 122,
                Currency = Currency.USD,
                Timestamp = DateTimeOffset.Now
            },
            new Voyage
            {
                VoyageCode = "451S",
                Price = 127,
                Currency = Currency.USD,
                Timestamp = DateTimeOffset.Now
            },
            new Voyage
            {
                VoyageCode = "451S",
                Price = 129,
                Currency = Currency.USD,
                Timestamp = DateTimeOffset.Now
            },
            new Voyage
            {
                VoyageCode = "451S",
                Price = 121,
                Currency = Currency.USD,
                Timestamp = DateTimeOffset.Now
            }
        };
        private readonly ILogger<VoyageController> _logger;
        private const string voyageListCacheKey = "voyageList";
        private IMemoryCache _cache;

        public VoyageController(IMemoryCache cache, ILogger<VoyageController> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync() 
        {
            _logger.Log(LogLevel.Information, "Trying to fetch the list of voyages from cache.");
            if (_cache.TryGetValue(voyageListCacheKey, out IEnumerable<Voyage> voyages))
            {
                _logger.Log(LogLevel.Information, "Voyages list found in cache.");
            }
            else
            {
                _logger.Log(LogLevel.Information, "Voyages list not found in cache. Fetching from database.");
                voyages = voyageList;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                        .SetPriority(CacheItemPriority.Normal)
                        .SetSize(1024);
                _cache.Set(voyageListCacheKey, voyages, cacheEntryOptions);
            }

            return Ok(voyages);
        }


        [HttpGet("GetAverage/{voyageCode}/{currency}")]
        public async Task<IActionResult> GetAverage(string voyageCode, Currency currency)
        {
            if (_cache.TryGetValue(voyageListCacheKey, out IEnumerable<Voyage> voyages))
            {
                _logger.Log(LogLevel.Information, "Voyages list found in cache.");
            }
            else
                voyages = voyageList;


            var filteredValues = voyages.Where(voyage => voyage.VoyageCode.Equals(voyageCode));
            if (filteredValues.Any())
            {
                var average = voyages.Sum(value => value.Price) / voyages.Count();
                return Ok(average);
            }

            return NotFound();
        }
    }
}
