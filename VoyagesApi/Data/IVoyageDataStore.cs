using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoyagesApi.Models;

namespace VoyagesApi.Data
{
    public interface IVoyageDataStore
    {
        Task<IEnumerable<Voyage>> GetAll();
        Task<(bool, IEnumerable<Voyage>)> SaveVoyage(Voyage voyage);
    }
}
