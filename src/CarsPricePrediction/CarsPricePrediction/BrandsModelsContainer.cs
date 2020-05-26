using System;
using System.Collections.Generic;
using System.Text;

namespace CarsPricePrediction
{
    public class BrandsModelsContainer
    {
        public Dictionary<string, List<string>> BrandsModels;

        public BrandsModelsContainer()
        {
            this.BrandsModels = new Dictionary<string, List<string>>();
        }
    }
}
