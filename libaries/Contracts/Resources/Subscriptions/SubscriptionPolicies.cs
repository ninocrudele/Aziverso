// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

namespace AOC.AzureCostsLibrary.API.Contracts.Resources.Subscriptions
{
    public class SubscriptionPolicies
    {
        [AzureAPIProperty("locationPlacementId", "", false)]
        public string locationPlacementId { get; set; }

        [AzureAPIProperty("quotaId", "", false)]
        public string quotaId { get; set; }

        [AzureAPIProperty("spendingLimit", "", false)]
        public string spendingLimit { get; set; }
    }
}