using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CarsDataClearer
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] carsInfo = File.ReadAllLines("data.csv");

            foreach (var item in carsInfo)
            {
                var carsInfoSplit = item.Split(",");
                bool isValid = true;
                for (int i = 8; i <= 24; i++)
                {
                    if (carsInfoSplit[i] == "-1")
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    File.AppendAllText("cleanData.csv", item + Environment.NewLine, Encoding.UTF8);
                }
            }
            
        }
    }
}
