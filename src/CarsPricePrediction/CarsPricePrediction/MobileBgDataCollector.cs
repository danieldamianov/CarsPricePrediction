﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using AngleSharp.Html.Parser;
using System.Net.Http;
using System.Text.RegularExpressions;

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

            const string pagesCountRegex = @"<b>Страница 1 от (?<pagesCount>\d{1,2})<\/b>";
            var pagesCountRegexObject = new System.Text.RegularExpressions.Regex(pagesCountRegex);

            var brandsModelsContainer = GetBrandsModelsContainer();

            foreach (var brand in brandsModelsContainer.BrandsModels)
            {
                foreach (var model in brand.Value)
                {
                    
                    var formData =
                    $"rub=1&act=3&f5={brand.Key}&f6={model}";
                    var response = await client.PostAsync(
                                       SearchAddressPost,
                                       new StringContent(formData, Encoding.UTF8, "application/x-www-form-urlencoded"));

                    var location = response.Headers.Location;

                    var carsFirstPage = await client.GetAsync(location);

                    var byteContent = await carsFirstPage.Content.ReadAsByteArrayAsync();
                    var html = Encoding.GetEncoding("windows-1251").GetString(byteContent);

                    var parssedHtml = await parser.ParseDocumentAsync(html);

                    var pagesElement = parssedHtml.GetElementsByClassName("pageNumbersInfo");
                    var innerText = pagesElement.First().InnerHtml;
                    pagesCountRegexObject.Match(innerText).Groups.TryGetValue("pagesCount",out Group group);
                    var totalPagesCount = int.Parse(group.Value);

                    for (int pageNumber = 1; pageNumber <= totalPagesCount; pageNumber++)
                    {
                        var pageLocation = @"https://www.mobile.bg" + location.PathAndQuery.Substring(0, location.PathAndQuery.Length - 1) + pageNumber;
                        var pageResponse = await client.GetAsync(pageLocation);

                        var byteContentPageResponse = await pageResponse.Content.ReadAsByteArrayAsync();
                        var htmlPageResponse = Encoding.GetEncoding("windows-1251").GetString(byteContent);

                        var parssedHtmlPageResponse = await parser.ParseDocumentAsync(htmlPageResponse);

                        var advertisements = parssedHtmlPageResponse.GetElementsByClassName("photoLink").Where(
                        x => x.Attributes["href"]?.Value?.Contains(@"//www.mobile.bg/pcgi/mobile.cgi?act=4&adv=") == true).ToList();

                        foreach (var advertisement in advertisements)
                        {
                            var url = "https:" + advertisement.Attributes["href"].Value;
                            var advertisementResponse = await client.GetAsync(url);

                            var byteContentAdvertisemen = await advertisementResponse.Content.ReadAsByteArrayAsync();
                            var htmlAdvertisement = Encoding.GetEncoding("windows-1251").GetString(byteContentAdvertisemen);

                            var advertisementPageParsed = await parser.ParseDocumentAsync(htmlAdvertisement);

                            var dillarDataElement = advertisementPageParsed.GetElementsByClassName("dilarData").First();
                            var listItemsInTheDillarData = dillarDataElement.GetElementsByTagName("li");
                            ;
                        }
                    }

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
