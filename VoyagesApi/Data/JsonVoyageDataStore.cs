using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VoyagesApi.Models;

namespace VoyagesApi.Data
{
    public class JsonVoyageDataStore : IVoyageDataStore
    {
        private string _jsonFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\Data.json");
        public IEnumerable<Voyage> GetAll()
        {
            var data = JsonConvert.DeserializeObject<IEnumerable<Voyage>>(File.ReadAllText(_jsonFilePath));
            return data;
        }

        public (bool, IEnumerable<Voyage>) SaveVoyage(Voyage voyage)
        {
            var newData = new List<Voyage>(); 
            var data = JsonConvert.DeserializeObject<IEnumerable<Voyage>>(File.ReadAllText(_jsonFilePath));
            newData.AddRange(data);
            newData.Add(voyage);
            string json = JsonConvert.SerializeObject(newData, Formatting.Indented);
            File.WriteAllText(_jsonFilePath, json);
            return (true, newData);
        }
    }
}
