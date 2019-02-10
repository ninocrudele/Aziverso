// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using System;

#endregion

namespace AOC.AzureCostsLibrary.API.Contracts.Resources.UsageDetails
{
    public class Properties
    {
        [AzureAPIProperty("billingPeriodId", "", false)]
        public string
            billingPeriodId
        {
            get;
            set;
        } // ": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/providers/Microsoft.Billing/billingPeriods/20181001",

        [AzureAPIProperty("usageStart", "", false)]
        public DateTime usageStart { get; set; } //": "2018-10-22T00:00:00.0000000Z",

        [AzureAPIProperty("usageEnd", "", false)]
        public DateTime usageEnd { get; set; } //": "2018-10-22T23:59:59.0000000Z",

        [AzureAPIProperty("instanceId", "", false)]
        public string
            instanceId
        {
            get;
            set;
        } //": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/resourceGroups/MI-BI-RG/providers/Microsoft.Storage/storageAccounts/mibirgdiag592",

        [AzureAPIProperty("instanceName", "", false)]
        public string instanceName { get; set; } //": "mibirgdiag592",

        [AzureAPIProperty("instanceLocation", "", false)]
        public string instanceLocation { get; set; } //": "euwest",

        [AzureAPIProperty("meterId", "", false)]
        public string meterId { get; set; } //": "923978e1-fd3f-4bd5-a798-f4b533057e46",

        [AzureAPIProperty("usageQuantity", "", false)]
        public double usageQuantity { get; set; } //": 0.2879,

        [AzureAPIProperty("pretaxCost", "", false)]
        public double pretaxCost { get; set; } //": 0.000074456896552,

        [AzureAPIProperty("currency", "", false)]
        public string currency { get; set; } //": "EUR",

        [AzureAPIProperty("isEstimated", "", false)]
        public bool isEstimated { get; set; } //": false,

        [AzureAPIProperty("subscriptionGuid", "", false)]
        public string subscriptionGuid { get; set; } //": "12ba10cc-a5b2-4bc7-a5ae-615729c05167",

        [AzureAPIProperty("subscriptionName", "", false)]
        public string subscriptionName { get; set; } //": "Hexagon MI BI",

        [AzureAPIProperty("accountName", "", false)]
        public string accountName { get; set; } //": "HEXAGON MI",

        [AzureAPIProperty("departmentName", "", false)]
        public string departmentName { get; set; } //": "MI-IT",

        [AzureAPIProperty("product", "", false)]
        public string product { get; set; } //": "General Block Blob - Delete Operations",

        [AzureAPIProperty("consumedService", "", false)]
        public string consumedService { get; set; } //": "Microsoft.Storage",

        [AzureAPIProperty("costCenter", "", false)]
        public string costCenter { get; set; } //": "Hexagon MI",

        [AzureAPIProperty("partNumber", "", false)]
        public string partNumber { get; set; } //": "N9H-00741",

        [AzureAPIProperty("resourceGuid", "", false)]
        public string resourceGuid { get; set; } //": "923978e1-fd3f-4bd5-a798-f4b533057e46",

        [AzureAPIProperty("offerId", "", false)]
        public string offerId { get; set; } //": "MS-AZR-0017P",

        [AzureAPIProperty("chargesBilledSeparately", "", false)]
        public bool chargesBilledSeparately { get; set; } //": false,

        [AzureAPIProperty("location", "", false)]
        public string location { get; set; } //": "Unassigned",

        [AzureAPIProperty("meterDetails", "", false)]
        public string meterDetails { get; set; } //": null

        public string internalDepartment { get; set; } //": null
    }
}