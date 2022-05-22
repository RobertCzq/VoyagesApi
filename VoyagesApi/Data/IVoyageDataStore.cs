using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyagesApi.Models;

namespace VoyagesApi.Data
{
    public interface IVoyageDataStore
    {
        IEnumerable<Voyage> GetAll();
        (bool, IEnumerable<Voyage>) SaveVoyage(Voyage voyage);
    }
}
