using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using VoyagesApi.Data;
using VoyagesApi.Models;
using VoyagesApi.Utils;

namespace VoyagesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    public class VoyagesController : ControllerBase
    {
        private readonly ILogger<VoyagesController> _logger;
        private const string voyageListCacheKey = "voyageList";
        private IMemoryCache _cache;
        private IVoyageDataStore _voyageData;

        public VoyagesController(IMemoryCache cache, ILogger<VoyagesController> logger, IVoyageDataStore voyageData)
        {
            _cache = cache;
            _logger = logger;
            _voyageData = voyageData;
        }

        /// <summary>
        /// Returns all voyage items
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetAll()
        {
            var voyages = GetVoyages();
            if(voyages.Any())
                return Ok(voyages);

            return NotFound();
        }

        /// <summary>
        /// Returns the average of all the prices for the specified voyage code in the specified currency 
        /// </summary>
        /// <param name="voyageCode"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpGet("GetAverage({voyageCode},{currency})")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetAverage(string voyageCode, Currency currency)
        {
            var voyages = GetVoyages();

            var filteredValues = voyages.Where(voyage => voyage.VoyageCode.Equals(voyageCode));
            if (filteredValues.Any())
            {
                var average = voyages.Sum(value => value.Price) / voyages.Count();
                return Ok(average);
            }

            return NotFound();
        }

        /// <summary>
        /// Adds a new record with the specified price, currency and timestamp for the voyage  
        /// </summary>
        /// <param name="voyageCode"></param>
        /// <param name="price"></param>
        /// <param name="currency"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [HttpPost("UpdatePrice({voyageCode},{price},{currency},{timestamp})")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult UpdatePrice(string voyageCode, decimal price, Currency currency, DateTimeOffset timestamp)
        {
            var newVoyage = new Voyage
            {
                VoyageCode = voyageCode,
                Price = price,
                Currency = currency,
                Timestamp = timestamp
            };

            var (saved, data) = _voyageData.SaveVoyage(newVoyage);
            if (saved)
            {
                CacheHelper.SetUpCache(voyageListCacheKey, _cache, data);
                return Created("", newVoyage);
            }

            return BadRequest();
        }

        private IEnumerable<Voyage> GetVoyages()
        {
            _logger.Log(LogLevel.Information, "Trying to fetch the list of voyages from cache.");
            if (_cache.TryGetValue(voyageListCacheKey, out IEnumerable<Voyage> voyages))
            {
                _logger.Log(LogLevel.Information, "Voyages list found in cache.");
            }
            else
            {
                _logger.Log(LogLevel.Information, "Voyages list not found in cache. Fetching from database.");
                voyages = GetData();
            }

            return voyages;
        }

        private IEnumerable<Voyage> GetData()
        {
            var voyageList = _voyageData.GetAll();
            CacheHelper.SetUpCache(voyageListCacheKey, _cache, voyageList);
            return voyageList;
        }

    }
}
