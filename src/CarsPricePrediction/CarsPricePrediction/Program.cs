using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CarsPricePrediction
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MobileBgDataCollector mobileBgDataCollector = new MobileBgDataCollector();
            await mobileBgDataCollector.CollectData();
            //MLContext mlContext = new MLContext();
            //IDataView data = mlContext.Data.LoadFromTextFile<DataModel>("data.csv", separatorChar: ',', hasHeader: true); ;
        }
    }
    class DataModel
    {
        [LoadColumn(0)]
        [ColumnName("Brand")]
        public string q { get; set; }
        [LoadColumn(1)]
        [ColumnName("Model")]
        public string w { get; set; }
        [LoadColumn(2)]
        [ColumnName("Category")]
        public string e { get; set; }
        [LoadColumn(3)]
        [ColumnName("ManufacturingDate")]
        public string r { get; set; }
        [LoadColumn(4)]
        [ColumnName("EngineType,Power")]
        public string t { get; set; }
        [LoadColumn(5)]
        [ColumnName("Shifter")]
        public string y { get; set; }
        [LoadColumn(6)]
        [ColumnName("DistanceTravelled")]
        public string u { get; set; }
        [LoadColumn(7)]
        [ColumnName("Въздушни възглавници - Предни")]
        public string i { get; set; }
        [LoadColumn(8)]
        [ColumnName("Бордкомпютър")]
        public string a { get; set; }
        [LoadColumn(9)]
        [ColumnName(@"Бързи \ бавни скорости")]
        public string we{ get; set; }
        [LoadColumn(10)]
        [ColumnName("Климатик")]
        public string s { get; set; }
        [LoadColumn(11)]
        [ColumnName("Климатроник")]
        public string d { get; set; }
        [LoadColumn(12)]
        [ColumnName("4x4")]
        public string f { get; set; }
        [LoadColumn(13)]
        [ColumnName("7 места")]
        public string g { get; set; }
        [LoadColumn(14)]
        [ColumnName("Газова уредба")]
        public string h { get; set; }
        [LoadColumn(15)]
        [ColumnName("Метанова уредба")]
        public string j { get; set; }
        [LoadColumn(16)]
        [ColumnName("С регистрация")]
        public string k { get; set; }
        [LoadColumn(17)]
        [ColumnName("2(3) Врати")]
        public string l { get; set; }
        [LoadColumn(18)]
        [ColumnName("4(5) Врати")]
        public string z { get; set; }
        [LoadColumn(19)]
        [ColumnName("Панорамен люк")]
        public string x { get; set; }
        [LoadColumn(20)]
        [ColumnName("Теглич")]
        public string c { get; set; }
        [LoadColumn(21)]
        [ColumnName("Аларма")]
        public string v { get; set; }
        [LoadColumn(22)]
        [ColumnName("Кожен салон")]
        public string b { get; set; }
        [LoadColumn(23)]
        [ColumnName("Десен волан")]
        public string n { get; set; }
        [LoadColumn(24)]
        [ColumnName("Price")]
        public string m { get; set; }
        [LoadColumn(25)]
        [ColumnName("AdvertisementUrl")]
        public string sd { get; set; }
    }
}
