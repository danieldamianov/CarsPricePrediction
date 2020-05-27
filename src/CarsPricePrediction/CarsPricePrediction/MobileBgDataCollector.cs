using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Linq;
using AngleSharp.Html.Parser;
using System.Net.Http;

namespace CarsPricePrediction
{
    public class MobileBgDataCollector
    {
        public async Task<IEnumerable<CarAdvertisementModel>> CollectData()
        {
            const string SearchAddressPost = "https://www.mobile.bg/pcgi/mobile.cgi";

            var advertisementmodels = new List<CarAdvertisementModel>();

            var parser = new HtmlParser();
            var handler = new HttpClientHandler { AllowAutoRedirect = false, };
            var client = new HttpClient(handler);

            var brandsModelsContainer = GetBrandsModelsContainer();

            foreach (var brand in brandsModelsContainer.BrandsModels)
            {
                foreach (var model in brand.Value)
                {
                    var formData =
                    $"topmenu=1&rub=1&act=3&rub_pub_save=1&f0=127.0.0.1&f1=1&f2=1";
                    var response = await client.PostAsync(
                                       SearchAddressPost,
                                       new StringContent(formData, Encoding.UTF8, "application/x-www-form-urlencoded"));

                    var location = response.Headers.Location;

                    Console.WriteLine($"Collection information about : {brand.Key} , {model}");
                }
            }


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

        public int CreatedBeforeInMonths { get; set; }

        public string EngineType { get; set; }

        public string ShifterType { get; set; }

        public int Power { get; set; }

        public int DistanceTravelled { get; set; }

        public int Price { get; set; }
    }
}
