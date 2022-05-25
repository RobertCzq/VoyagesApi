using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all elements");
            var voyages = await GetVoyages();
            if (voyages.Any())
            {
                return Ok(voyages);
            }

            _logger.LogWarning("GetAll NOT FOUND");
            return NotFound();
        }

        /// <summary>
        /// Returns the average of the last 10 prices for the specified voyage code in the specified currency 
        /// </summary>
        /// <param name="voyageCode"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        [HttpGet("GetAveragePrice({voyageCode},{currency})")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Administrator,Normal")]
        public async Task<IActionResult> GetAveragePrice(string voyageCode, Currency currency)
        {
            _logger.LogInformation("Getting averege price for voyage code {0} in currency {1}", voyageCode, currency);
            var voyages = await GetVoyages();
            var filteredVoyages = voyages.Where(voyage => voyage.VoyageCode.Equals(voyageCode));
            if (filteredVoyages.Any())
            {
                var totalNrOfVoyages = filteredVoyages.Count();
                var nrItemsToTake = 10;
                var toDivideTo = totalNrOfVoyages >= nrItemsToTake ? nrItemsToTake : totalNrOfVoyages; 
                var averagePrice = filteredVoyages.OrderByDescending(v => v.Timestamp)
                                     .Take(nrItemsToTake)
                                     .Sum(value => CurrencyHelper.TransformToCurrency(value, currency)) / toDivideTo;
                return Ok(averagePrice);
            }

            _logger.LogInformation("Get averege price NOT FOUND for voyage code {0} in currency {1}", voyageCode, currency);
            return NotFound();
        }

        /// <summary>
        /// Adds a new price for the voyage   
        /// </summary>
        /// <param name="voyageCode"></param>
        /// <param name="price"></param>
        /// <param name="currency"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        [HttpPost("UpdatePrice({voyageCode},{price},{currency},{timestamp})")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdatePrice(string voyageCode, decimal price, Currency currency, DateTimeOffset timestamp)
        {
            _logger.LogInformation("Add new price for voyage code {0} in currency {1} at time {3}", voyageCode, currency, timestamp);


            var (saved, voyage, data) = await _voyageData.SaveVoyagePrice(voyageCode, price, currency, timestamp);
            if (saved)
            {
                CacheHelper.SetUpCache(voyageListCacheKey, _cache, data);
                return Created("", voyage);
            }

            _logger.LogInformation("Add new price BAD REQUEST for voyage code {0} in currency {1} at time {3}", voyageCode, currency, timestamp);
            return BadRequest();
        }

        /// <summary>
        /// Adds a new price for the voyage using the FromBody voyage object 
        /// </summary>
        /// <param name="newVoyage"></param>
        /// <returns></returns>
        [HttpPost("UpdatePrice")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdatePrice([FromBody] Voyage newVoyage)
        {
            _logger.LogInformation("Add new price for voyage code {0} in currency {1} at time {3}", newVoyage.VoyageCode, newVoyage.Currency, newVoyage.Timestamp);

            var (saved, _, data) = await _voyageData.SaveVoyagePrice(newVoyage.VoyageCode, newVoyage.Price, newVoyage.Currency, newVoyage.Timestamp);
            if (saved)
            {
                CacheHelper.SetUpCache(voyageListCacheKey, _cache, data);
                return Created("", newVoyage);
            }

            _logger.LogInformation("Add new price BAD REQUEST for voyage code {0} in currency {1} at time {3}", newVoyage.VoyageCode, newVoyage.Currency, newVoyage.Timestamp);
            return BadRequest();
        }

        private async Task<IEnumerable<Voyage>> GetVoyages()
        {
            _logger.Log(LogLevel.Information, "Trying to fetch the list of voyages from cache.");
            if (_cache.TryGetValue(voyageListCacheKey, out IEnumerable<Voyage> voyages))
            {
                _logger.Log(LogLevel.Information, "Voyages list found in cache.");
            }
            else
            {
                _logger.Log(LogLevel.Information, "Voyages list not found in cache. Fetching from database.");
                voyages = await GetData();
            }

            return voyages;
        }

        private async Task<IEnumerable<Voyage>> GetData()
        {
            var voyageList = await _voyageData.GetAll();
            CacheHelper.SetUpCache(voyageListCacheKey, _cache, voyageList);
            return voyageList;
        }

    }
}
