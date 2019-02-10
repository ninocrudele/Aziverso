// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

namespace AOC.AzureCostsLibrary.API.Contracts.Resources.RG
{
    public class value
    {
        [AzureAPIProperty("id", "", false)] public string id { get; set; }

        [AzureAPIProperty("name", "", false)] public string name { get; set; }
    }
}