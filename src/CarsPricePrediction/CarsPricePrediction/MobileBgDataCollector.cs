namespace CarsPricePrediction
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;
    using System.Linq;
    using AngleSharp.Html.Parser;
    using System.Net.Http;
    using System.Text.RegularExpressions;
    using AngleSharp.Dom;
    using AngleSharp.Html.Dom;

    public class MobileBgDataCollector
    {
        private readonly HtmlParser parser;
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;

        public MobileBgDataCollector()
        {
            this.parser = new HtmlParser();
            this.handler = new HttpClientHandler { AllowAutoRedirect = false, };
            this.client = new HttpClient(handler);
        }

        private async Task<IHtmlDocument> GetParsedHtml(HttpResponseMessage httpResponse)
        {
            var byteContent = await httpResponse.Content.ReadAsByteArrayAsync();
            var htmlRaw = Encoding.GetEncoding("windows-1251").GetString(byteContent);
            var parsedHtml = await parser.ParseDocumentAsync(htmlRaw);
            return parsedHtml;
        }

        public async Task CollectData()
        {
            int skippedBecauseLackAdditionalInfo = 0;
            int skippedBecauseMainInfo = 0;

            string[] aditionalInfoFeatrures = new string[] { "Въздушни възглавници - Предни", "Бордкомпютър", @"Бързи \ бавни скорости",
                "Климатик", "Климатроник", "4x4", "7 места", "Газова уредба", "Дълга база", "Къса база", "Метанова уредба", "С регистрация",
                "2(3) Врати", "4(5) Врати", "Панорамен люк", "Теглич", "Аларма", "Кожен салон", "Десен волан", 
            };
            File.WriteAllText("data.csv", "Brand,", Encoding.GetEncoding(1251));
            File.AppendAllText("data.csv", "Model,");
            File.AppendAllText("data.csv", "Category,");
            File.AppendAllText("data.csv", "ManufacturingDate,");
            File.AppendAllText("data.csv", "EngineType,");
            File.AppendAllText("data.csv", "Power,");
            File.AppendAllText("data.csv", "Shifter,");
            File.AppendAllText("data.csv", "DistanceTravelled,");
            foreach (var featrue in aditionalInfoFeatrures)
            {
                File.AppendAllText("data.csv", $"{featrue},", Encoding.GetEncoding(1251));
            }
            File.AppendAllText("data.csv", "AdvertisementUrl,");
            File.AppendAllText("data.csv", Environment.NewLine);

            const string SearchAddressPost = "https://www.mobile.bg/pcgi/mobile.cgi";
            const string pagesCountRegex = @"<b>Страница 1 от (?<pagesCount>\d{1,2})<\/b>";
            var pagesCountRegexObject = new Regex(pagesCountRegex);

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

                    var parssedHtml = await GetParsedHtml(carsFirstPage);

                    var pagesElement = parssedHtml.GetElementsByClassName("pageNumbersInfo");
                    var innerText = pagesElement.First().InnerHtml;
                    pagesCountRegexObject.Match(innerText).Groups.TryGetValue("pagesCount", out Group group);
                    var totalPagesCount = int.Parse(group.Value);

                    for (int pageNumber = 1; pageNumber <= totalPagesCount; pageNumber++)
                    {
                        var pageLocation = @"https://www.mobile.bg" + location.PathAndQuery.Substring(0, location.PathAndQuery.Length - 1) + pageNumber;
                        var pageResponse = await client.GetAsync(pageLocation);

                        var parssedHtmlPageResponse = await this.GetParsedHtml(pageResponse);

                        var advertisements = parssedHtmlPageResponse.GetElementsByClassName("photoLink").Where(
                        x => x.Attributes["href"]?.Value?.Contains(@"//www.mobile.bg/pcgi/mobile.cgi?act=4&adv=") == true).ToList();

                        foreach (var advertisement in advertisements)
                        {
                            var url = "https:" + advertisement.Attributes["href"].Value;
                            var advertisementResponse = await client.GetAsync(url);

                            var advertisementPageParsed = await this.GetParsedHtml(advertisementResponse);

                            var dillarDataElement = advertisementPageParsed.GetElementsByClassName("dilarData").First();
                            var listItemsInTheDillarData = dillarDataElement.GetElementsByTagName("li");

                            Dictionary<string, string> mainProperties = new Dictionary<string, string>();

                            for (int i = 0; i < listItemsInTheDillarData.Length; i += 2)
                            {
                                mainProperties.Add(listItemsInTheDillarData[i].InnerHtml, listItemsInTheDillarData[i + 1].InnerHtml);
                            }

                            if ((!mainProperties.ContainsKey("Дата на производство"))
                                || (!mainProperties.ContainsKey("Тип двигател"))
                                || (!mainProperties.ContainsKey("Мощност"))
                                || (!mainProperties.ContainsKey("Скоростна кутия"))
                                || (!mainProperties.ContainsKey("Категория"))
                                || (!mainProperties.ContainsKey("Пробег"))
                                )
                            {
                                skippedBecauseMainInfo++;
                                continue;
                            }

                            File.AppendAllText("data.csv", $"{brand.Key},".Trim());
                            File.AppendAllText("data.csv", $"{model},".Trim());
                            File.AppendAllText("data.csv", $"{mainProperties["Категория"]},".Trim(), Encoding.GetEncoding(1251));
                            File.AppendAllText("data.csv", $"{mainProperties["Дата на производство"]},".Trim(), Encoding.GetEncoding(1251));
                            File.AppendAllText("data.csv", $"{mainProperties["Тип двигател"]},".Trim(), Encoding.GetEncoding(1251));
                            File.AppendAllText("data.csv", $"{mainProperties["Мощност"]},".Trim(), Encoding.GetEncoding(1251));
                            File.AppendAllText("data.csv", $"{mainProperties["Скоростна кутия"]},".Trim(), Encoding.GetEncoding(1251));
                            File.AppendAllText("data.csv", $"{mainProperties["Пробег"]},".Trim(), Encoding.GetEncoding(1251));

                            IHtmlCollection<IElement> tables = advertisementPageParsed.GetElementsByTagName("table");

                            var tableInfo = tables
                                .FirstOrDefault(el => el.InnerHtml.Contains("</table>") == false && (el.InnerHtml.Contains(@"<label class=""extra_cat"">Безопасност</label>")
                                || el.InnerHtml.Contains(@"<label class=""extra_cat"">Комфорт</label>")
                                || el.InnerHtml.Contains(@"<label class=""extra_cat"">Други</label>")
                                || el.InnerHtml.Contains(@"<label class=""extra_cat"">Защита</label>")
                                || el.InnerHtml.Contains(@"<label class=""extra_cat"">Интериор</label>")
                                || el.InnerHtml.Contains(@"<label class=""extra_cat"">Екстериор</label>")
                                || el.InnerHtml.Contains(@"<label class=""extra_cat"">Специализирани</label>")));

                            if (tableInfo == null)
                            {
                                for (int i = 0; i < aditionalInfoFeatrures.Length; i++)
                                {
                                    File.AppendAllText("data.csv", $"NULL,");
                                }
                                skippedBecauseLackAdditionalInfo++;
                            }
                            else
                            {
                                foreach (var featrue in aditionalInfoFeatrures)
                                {
                                    File.AppendAllText("data.csv", $"{tableInfo.InnerHtml.Contains(featrue)},", Encoding.GetEncoding(1251));
                                }
                            }

                            File.AppendAllText("data.csv", $"{url},".Trim(), Encoding.GetEncoding(1251));
                            File.AppendAllText("data.csv", Environment.NewLine);
                        }
                    }

                    Console.WriteLine($"Collection information about : {brand.Key} , {model}");
                    if (brand.Key == "Alfa Romeo" && model == "146")
                    {
                        Console.WriteLine(nameof(skippedBecauseLackAdditionalInfo) + " " + skippedBecauseLackAdditionalInfo);
                        Console.WriteLine(nameof(skippedBecauseMainInfo) + " " + skippedBecauseMainInfo);
                        return;
                    }
                }
            }

            Console.WriteLine(nameof(skippedBecauseLackAdditionalInfo) + " " + skippedBecauseLackAdditionalInfo);
            Console.WriteLine(nameof(skippedBecauseMainInfo) + " " + skippedBecauseMainInfo);
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
}
