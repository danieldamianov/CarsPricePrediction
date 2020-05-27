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
        }
    }
}
