using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoyagesApi.Models
{
    public class Voyage
    {
        public string VoyageCode { get; set; }
        public decimal Price { get; set; }
        public Currency Currency { get; set; }
        public DateTimeOffset Timestamp { get; set; } 
    }

    public enum Currency
    {
        USD,
        EUR,
        GBP
    }
}
