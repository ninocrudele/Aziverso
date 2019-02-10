// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using System.Collections.Generic;

#endregion

namespace AOC.AzureCostsLibrary.API.Contracts.Costs.PriceSheet
{
    public class DataPayload
    {
        public List<Value> values { get; set; }
    }
}