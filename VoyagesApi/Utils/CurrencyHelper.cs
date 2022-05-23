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
            { Tuple.Create(Currency.USD, Currency.EUR), 0.94m },
            { Tuple.Create(Currency.USD, Currency.GBP), 0.79m },
            { Tuple.Create(Currency.EUR, Currency.USD), 1.07m },
            { Tuple.Create(Currency.EUR, Currency.GBP), 0.85m },
            { Tuple.Create(Currency.GBP, Currency.EUR), 1.18m },
            { Tuple.Create(Currency.GBP, Currency.USD), 1.26m },
            { Tuple.Create(Currency.EUR, Currency.EUR), 1 },
            { Tuple.Create(Currency.GBP, Currency.GBP), 1 },
            { Tuple.Create(Currency.USD, Currency.USD), 1 }
        };

        public static decimal TransformToCurrency(Voyage voyage, Currency toCurrency)
        {
            var key = Tuple.Create(voyage.Currency, toCurrency);
            if (conversionRates.ContainsKey(key))
            {
                var conversionRate = conversionRates[key];
                return (voyage.Price * conversionRate);
            }
            //TODO add logging for something wrong here
            return 0;
        }

    }
}
