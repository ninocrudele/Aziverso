// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

namespace AOC.AzureCostsLibrary.API.Contracts.Costs.UsageDetails
{
    public class UsageDetailBillingDatasetToQuery
    {
        public string instanceLocation { get; set; }
        public string instanceId { get; set; }
        public string instanceName { get; set; }
        public double usageQuantity { get; set; }
        public double pretaxCost { get; set; }
        public string currency { get; set; }
        public string tags { get; set; }
        public string tagValue { get; set; }
        public string resouceGroup { get; set; }
        public string subscriptionId { get; set; }
        public string subscriptionName { get; set; }
        public string accountName { get; set; }
        public string departmentName { get; set; }
        public string productOriginal { get; set; }
        public string product { get; set; }
        public string consumedService { get; set; }
        public string costCenter { get; set; }
        public string partNumber { get; set; }
        public string offerId { get; set; }
        public string location { get; set; }
        public int countOf { get; set; }
    }
}