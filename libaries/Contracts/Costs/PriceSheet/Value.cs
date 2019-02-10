// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

namespace AOC.AzureCostsLibrary.API.Contracts.Costs.PriceSheet
{
    public class Value
    {
        [AzureAPIProperty("id", "", false)] public string id { get; set; }

        [AzureAPIProperty("billingPeriodId", "", false)]
        public string billingPeriodId { get; set; }

        [AzureAPIProperty("meterId", "", false)]
        public string meterId { get; set; }

        [AzureAPIProperty("meterName", "", false)]
        public string meterName { get; set; }

        [AzureAPIProperty("unitOfMeasure", "", false)]
        public string unitOfMeasure { get; set; }

        [AzureAPIProperty("includedQuantity", "", false)]
        public string includedQuantity { get; set; }

        [AzureAPIProperty("partNumber", "", false)]
        public string partNumber { get; set; }

        [AzureAPIProperty("unitPrice", "", false)]
        public string unitPrice { get; set; }

        [AzureAPIProperty("currencyCode", "", false)]
        public string currencyCode { get; set; }
    }
}