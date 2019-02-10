// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using System.Collections.Generic;

#endregion

namespace AOC.AzureOfficeCompanion.WorkSheet
{
    public class DataSetEnlistObject
    {
        public DataSetEnlistObject(object[,] dataSetArray, Dictionary<string, int> heuristicWords)
        {
            DataSetArray = dataSetArray;
            HeuristicWords = heuristicWords;
        }

        public object[,] DataSetArray { get; set; }
        public Dictionary<string, int> HeuristicWords { get; set; }
    }
}