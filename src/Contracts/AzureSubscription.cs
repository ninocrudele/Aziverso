// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using AOC.AzureCostsLibrary.API.Contracts;

#endregion

namespace AOC.AzureOfficeCompanion.Contracts
{
    public class AzureSubscription
    {
        public AzureSubscription(string name, string id, string department, double totalBibbling)
        {
            Name = name;
            Id = id;
            Department = department;
            TotalBibbling = totalBibbling;
        }

        [AzureAPIProperty("Department", "", false)]
        public string Department { get; set; }

        [AzureAPIProperty("Subscription Name", "", false)]
        public string Name { get; set; }

        [AzureAPIProperty("Subscription ID", "", false)]
        public string Id { get; set; }

        [AzureAPIProperty("Total cost", "", false)]
        public double TotalBibbling { get; set; }
    }

    public class AzureSubscriptionTotal
    {
        public AzureSubscriptionTotal(string name, string id, string department, double totalBibbling)
        {
            Name = name;
            Id = id;
            Department = department;
            TotalBibbling = totalBibbling;
        }

        [AzureAPIProperty("Department", "", false)]
        public string Department { get; set; }

        [AzureAPIProperty("Billing Period", "", false)]
        public string Name { get; set; }

        [AzureAPIProperty("Subscription ID", "", false)]
        public string Id { get; set; }

        [AzureAPIProperty("Total cost", "", false)]
        public double TotalBibbling { get; set; }
    }

    public class AzureResourceGroup
    {
        public AzureResourceGroup(string rgname, string subname, string subid, string department)
        {
            RGName = rgname;
            SubName = subname;
            SubId = subid;
            Department = department;
        }

        public string Department { get; set; }
        public string SubName { get; set; }
        public string SubId { get; set; }
        public string RGName { get; set; }
    }
}