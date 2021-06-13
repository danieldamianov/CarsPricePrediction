// This file was auto-generated by ML.NET Model Builder. 

using System;
using CarsDataClearerML.Model;

namespace CarsDataClearerML.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = new ModelInput()
            {
                Brand = @"Abarth",
                Model = @"595",
                Category = @"Купе",
                MonthsSinceManufacturing = 45F,
                EngineType = @"Бензинов",
                Power = 180F,
                Shifter = @"Ръчна",
                DistanceTravelled = 96000F,
                Въздушни_възглавници___Предни = 0F,
                Бордкомпютър = 1F,
                Бързи_бавни_скорости = 0F,
                Климатик = 0F,
                Климатроник = 1F,
                _4x4 = 0F,
                _7_места = 0F,
                Газова_уредба = 0F,
                Метанова_уредба = 0F,
                С_регистрация = 0F,
                _2_3__Врати = 1F,
                _4_5__Врати = 0F,
                Панорамен_люк = 0F,
                Теглич = 0F,
                Аларма = 0F,
                Кожен_салон = 0F,
                Десен_волан = 0F,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual Price with predicted Price from sample data...\n\n");
            Console.WriteLine($"Brand: {sampleData.Brand}");
            Console.WriteLine($"Model: {sampleData.Model}");
            Console.WriteLine($"Category: {sampleData.Category}");
            Console.WriteLine($"MonthsSinceManufacturing: {sampleData.MonthsSinceManufacturing}");
            Console.WriteLine($"EngineType: {sampleData.EngineType}");
            Console.WriteLine($"Power: {sampleData.Power}");
            Console.WriteLine($"Shifter: {sampleData.Shifter}");
            Console.WriteLine($"DistanceTravelled: {sampleData.DistanceTravelled}");
            Console.WriteLine($"Въздушни_възглавници___Предни: {sampleData.Въздушни_възглавници___Предни}");
            Console.WriteLine($"Бордкомпютър: {sampleData.Бордкомпютър}");
            Console.WriteLine($"Бързи_бавни_скорости: {sampleData.Бързи_бавни_скорости}");
            Console.WriteLine($"Климатик: {sampleData.Климатик}");
            Console.WriteLine($"Климатроник: {sampleData.Климатроник}");
            Console.WriteLine($"_4x4: {sampleData._4x4}");
            Console.WriteLine($"_7_места: {sampleData._7_места}");
            Console.WriteLine($"Газова_уредба: {sampleData.Газова_уредба}");
            Console.WriteLine($"Метанова_уредба: {sampleData.Метанова_уредба}");
            Console.WriteLine($"С_регистрация: {sampleData.С_регистрация}");
            Console.WriteLine($"_2_3__Врати: {sampleData._2_3__Врати}");
            Console.WriteLine($"_4_5__Врати: {sampleData._4_5__Врати}");
            Console.WriteLine($"Панорамен_люк: {sampleData.Панорамен_люк}");
            Console.WriteLine($"Теглич: {sampleData.Теглич}");
            Console.WriteLine($"Аларма: {sampleData.Аларма}");
            Console.WriteLine($"Кожен_салон: {sampleData.Кожен_салон}");
            Console.WriteLine($"Десен_волан: {sampleData.Десен_волан}");
            Console.WriteLine($"\n\nPredicted Price: {predictionResult.Score}\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
    }
}