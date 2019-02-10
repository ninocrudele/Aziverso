using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOC.AzureCostsLibrary.API.Interfaces
{
    /// <summary>
    /// Class used by the API Helper to return multiple data sets
    /// </summary>
    public class APIDataset
    {
        public object[,] ArrayDataSet { get; set; }
        public object DataPayLoad { get; set; }

        public APIDataset()
        {

        }
        public APIDataset(object[,] arrayDataSet, object dataPayLoad)
        {
            ArrayDataSet = arrayDataSet;
            DataPayLoad = dataPayLoad;
        }
    }
}
