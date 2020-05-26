using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace CarsPricePrediction
{
    public class MobileBgDataCollector
    {
        public async Task<IEnumerable<CarAdvertisementModel>> CollectData(int startPrice, int endPrice)
        {
            var advertisementmodels = new List<CarAdvertisementModel>();

            return advertisementmodels;
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
