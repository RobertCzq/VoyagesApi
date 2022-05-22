using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyagesApi.Models;

namespace VoyagesApi.Utils
{
    public static class CurrencyHelper
    {

        private static Dictionary<Tuple<Currency, Currency>, decimal> conversionRates
            = new Dictionary<Tuple<Currency, Currency>, decimal>           
        {
            { Tuple.Create(Currency.USD, Currency.EUR), 1.10m },
            { Tuple.Create(Currency.USD, Currency.GBX), 1.20m },
            { Tuple.Create(Currency.EUR, Currency.USD), 0.90m },
            { Tuple.Create(Currency.EUR, Currency.GBX), 1.10m },
            { Tuple.Create(Currency.GBX, Currency.EUR), 0.90m },
            { Tuple.Create(Currency.GBX, Currency.USD), 1.10m },
            { Tuple.Create(Currency.EUR, Currency.EUR), 1 },
            { Tuple.Create(Currency.GBX, Currency.GBX), 1 },
            { Tuple.Create(Currency.USD, Currency.USD), 1 }
        };

        public static decimal TransformToCurrency(Voyage voyage, Currency toCurrency)
        {
            var conversionRate = conversionRates[Tuple.Create(voyage.Currency, toCurrency)];
            return (voyage.Price * conversionRate);
        }

    }
}
