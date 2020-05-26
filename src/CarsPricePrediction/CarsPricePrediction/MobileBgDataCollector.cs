using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace CarsPricePrediction
{
    public class MobileBgDataCollector
    {
        public async Task<IEnumerable<CarAdvertisementModel>> CollectData(int startPrice, int endPrice)
        {
            var advertisementmodels = new List<CarAdvertisementModel>();

            var brandsModelsContainer = GetBrandsModelsContainer();

            return advertisementmodels;
        }

        private static BrandsModelsContainer GetBrandsModelsContainer()
        {
            var brandsModelsContainer = new BrandsModelsContainer();

            var lines = File.ReadAllLines("BrandsModels.txt");
            lines = lines.Select(line => line.Substring(1, line.Length - 2)).ToArray();
            var linesSplit = lines.Select(line => line.Split(",")).ToArray();

            for (int i = 0; i < linesSplit.Count(); i++)
            {
                linesSplit[i] = linesSplit[i].Select(str => str.Substring(1, str.Length - 2)).ToArray();
                brandsModelsContainer.BrandsModels.Add(linesSplit[i][0], linesSplit[i].Skip(2).ToList());
            }

            return brandsModelsContainer;
        }
    }

    public class CarAdvertisementModel
    {
        public string Brand { get; set; }

        public string Model { get; set; }

        // Джип, Хечбек, Седан ...
        public int Category { get; set; }

        public string YearOfManufacturing { get; set; }

        public bool IsNew { get; set; }

        public string EngineType { get; set; }

        public string ShifterType { get; set; }

        public int Power { get; set; }

        public int DistanceTravelled { get; set; }

        public int Price { get; set; }
    }
}
