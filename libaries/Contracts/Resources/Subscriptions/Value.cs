// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

namespace AOC.AzureCostsLibrary.API.Contracts.Resources.Subscriptions
{
    public class value
    {
        [AzureAPIProperty("id", "", false)] public string id { get; set; }

        [AzureAPIProperty("subscriptionId", "", false)]
        public string subscriptionId { get; set; }

        [AzureAPIProperty("displayName", "", false)]
        public string displayName { get; set; }

        [AzureAPIProperty("state", "", false)] public string state { get; set; }

        [AzureAPIProperty("authorizationSource", "", false)]
        public string authorizationSource { get; set; }

        [AzureAPIProperty("subscriptionPolicies", "", true)]
        public SubscriptionPolicies subscriptionPolicies { get; set; }
    }
}