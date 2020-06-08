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

        private int GetnumberFromMonthDependingOnCurrentMonth(string month)
        {
            switch (month)
            {
                case "януари":
                    return 5;
                case "февруари":
                    return 4;
                case "март":
                    return 3;
                case "април":
                    return 2;
                case "май":
                    return 1;
                case "юни":
                    return 0;
                case "юли":
                    return -1;
                case "август":
                    return -2;
                case "септември":
                    return -3;
                case "октомври":
                    return -4;
                case "ноември":
                    return -5;
                case "декември":
                    return -6;
                default:
                    throw new ArgumentException("Invalid month");
            }
        }

        public async Task CollectData()
        {
            int skippedBecauseLackAdditionalInfo = 0;
            int skippedBecauseMainInfo = 0;

            string[] aditionalInfoFeatrures = new string[] {
                "Въздушни възглавници - Предни", "Бордкомпютър", @"Бързи \ бавни скорости",
                "Климатик", "Климатроник", "4x4", "7 места", "Газова уредба", "Метанова уредба", "С регистрация",
                "2(3) Врати", "4(5) Врати", "Панорамен люк", "Теглич", "Аларма", "Кожен салон", "Десен волан",
            };
            File.WriteAllText("data1.csv", "Brand,", Encoding.UTF8);
            File.AppendAllText("data1.csv", "Model,");
            File.AppendAllText("data1.csv", "Category,");
            File.AppendAllText("data1.csv", "ManufacturingDate,");
            File.AppendAllText("data1.csv", "MonthsSinceManufacturing,");
            File.AppendAllText("data1.csv", "EngineType,");
            File.AppendAllText("data1.csv", "Power,");
            File.AppendAllText("data1.csv", "Shifter,");
            File.AppendAllText("data1.csv", "DistanceTravelled,");
            foreach (var featrue in aditionalInfoFeatrures)
            {
                File.AppendAllText("data1.csv", $"{featrue},", Encoding.UTF8);
            }
            File.AppendAllText("data1.csv", "Price,");
            File.AppendAllText("data1.csv", @"""AdvertisementUrl""");
            File.AppendAllText("data1.csv", "\n");

            const string SearchAddressPost = "https://www.mobile.bg/pcgi/mobile.cgi";
            const string pagesCountRegex = @"<b>Страница 1 от (?<pagesCount>\d{1,2})<\/b>";
            const string manufacturedTimeRegex = @"(?<month>[а-я]{2,})\s(?<year>[\d]{4})";

            var pagesCountRegexObject = new Regex(pagesCountRegex);
            var manufacturedTimeRegexObject = new Regex(manufacturedTimeRegex);

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

                            var priceDetailsElement = advertisementPageParsed.GetElementById("details_price");
                            int price = 0;
                            try
                            {
                                price = int.Parse(string.Join("", priceDetailsElement.InnerHtml.Substring(0, priceDetailsElement.InnerHtml.Length - 4)
                                                        .Trim()
                                                        .Select(ch => ch == ' ' ? "" : ch.ToString())));
                            }
                            catch (FormatException)
                            {   
                                //Handle "По договаряне"
                                continue;
                            }

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

                            File.AppendAllText("data1.csv", $"{brand.Key.Trim()},");
                            File.AppendAllText("data1.csv", $"{model.Trim()},");
                            File.AppendAllText("data1.csv", $"{mainProperties["Категория"].Trim()},", Encoding.UTF8);
                            File.AppendAllText("data1.csv", $"{mainProperties["Дата на производство"].Trim()},", Encoding.UTF8);
                            GroupCollection groups = manufacturedTimeRegexObject.Match(mainProperties["Дата на производство"].Trim()).Groups;
                            groups.TryGetValue("month", out Group monthGroup);
                            string month = monthGroup.Value;
                            groups.TryGetValue("year", out Group yearЯGroup);
                            string year = yearЯGroup.Value;
                            var monthsSinceManufacturing = (2020 - int.Parse(year)) * 12 + this.GetnumberFromMonthDependingOnCurrentMonth(month);
                            File.AppendAllText("data1.csv", $"{monthsSinceManufacturing},", Encoding.UTF8);
                            File.AppendAllText("data1.csv", $"{mainProperties["Тип двигател"].Trim()},", Encoding.UTF8);
                            File.AppendAllText("data1.csv", $"{mainProperties["Мощност"].Trim().Substring(0,mainProperties["Мощност"].Trim().Length - 5)},", Encoding.UTF8);
                            File.AppendAllText("data1.csv", $"{mainProperties["Скоростна кутия"].Trim()},", Encoding.UTF8);
                            File.AppendAllText("data1.csv", $"{mainProperties["Пробег"].Trim().Substring(0, mainProperties["Пробег"].Trim().Length - 3)},".Trim(), Encoding.UTF8);

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
                                    File.AppendAllText("data1.csv", $"-1,");
                                }
                                skippedBecauseLackAdditionalInfo++;
                            }
                            else
                            {
                                foreach (var featrue in aditionalInfoFeatrures)
                                {
                                    string trueOrFalseToInt = tableInfo.InnerHtml.Contains(featrue) ? "1" : "0";
                                    File.AppendAllText("data1.csv", $"{trueOrFalseToInt},", Encoding.UTF8);
                                }
                            }

                            File.AppendAllText("data1.csv", $"{price},".Trim(), Encoding.UTF8);
                            File.AppendAllText("data1.csv", @$"""{url}""".Trim(), Encoding.UTF8);
                            File.AppendAllText("data1.csv", "\n");
                        }
                    }

                    Console.WriteLine($"Collection information about : {brand.Key} , {model}");
                    if (brand.Key == "Alfa Romeo" && model == "147")
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
            lines = lines.Select(line => line.Substring(1, line.Length - 3)).ToArray();
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
