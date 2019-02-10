// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using System.Collections.Generic;

#endregion

namespace AOC.AzureCostsLibrary.API.Contracts.Resources.UsageDetails
{
    public class value
    {
        [AzureAPIProperty("id", "", false)] public string id { get; set; }

        [AzureAPIProperty("name", "", false)] public string name { get; set; }

        [AzureAPIProperty("type", "", false)] public string type { get; set; }

        [AzureAPIProperty("tags", "", false)] public IDictionary<string, string> tags { get; set; }

        [AzureAPIProperty("properties", "", true)]
        public Properties properties { get; set; }
    }
}