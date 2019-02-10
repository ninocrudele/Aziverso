// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using System.Collections.Generic;

#endregion

namespace AOC.AzureOfficeCompanion.Contracts
{
    internal class EnlistedResource
    {
        public EnlistedResource(string apiNumber, string groupNme, string title, string subCategory, string titleDescription, string version, string restCall, string restCallDescription, string fullyQualifyNameClass, List<string> parameters)
        {
            APINumber = apiNumber;
            GroupNme = groupNme;
            Title = title;
            SubCategory = subCategory;
            TitleDescription = titleDescription;
            Version = version;
            RestCall = restCall;
            RestCallDescription = restCallDescription;
            FullyQualifyNameClass = fullyQualifyNameClass;
            Parameters = parameters;
        }

        public string APINumber { get; set; }
        public string GroupNme { get; set; }
        public string Title { get; set; }
        public string SubCategory { get; set; }
        public string TitleDescription { get; set; }
        public string Version { get; set; }
        public string RestCall { get; set; }
        public string RestCallDescription { get; set; }
        public string FullyQualifyNameClass { get; set; }
        public List<string> Parameters { get; set; }
    }
}