using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AOC.AzureAPILibrary.API;
using AOC.AzureCostsLibrary.HelpClasses;

namespace AOC.ConsoleAppTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string orig = "Virtual Machines Ev3/ESv3 Series Windows - E4 v3/E4s v3  - EU West";


            //RunApp().Wait();

        }

        static async Task RunApp()
        {

            ColorGenerator colorGenerator = new ColorGenerator(Enumerable.Range(10000, 16384));

            for (int i = 0; i < 10000; i++)
            {


            }


            //UsageDetails usageLib = new UsageDetails("12ba10cc-a5b2-4bc7-a5ae-615729c05167");
            string token = APIHelpers.GetAccessToken();
            
            //string requestURL = $"https://management.azure.com/subscriptions?api-version=2016-06-01";
            //var taskOp = usageLib.GetDataObject(requestURL, "AOC.AzureAPILibrary.Resources.Data.DataPayload", token);
           
            string requestURL = $"https://management.azure.com/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/providers/Microsoft.Billing/billingPeriods/201810/providers/Microsoft.Consumption/usageDetails?api-version=2018-10-01&top=1";
            var taskOp = APIHelpers.GetDataObject(requestURL, "AOC.AzureAPILibrary.Costs.Data.DataPayload", token);
            taskOp.Wait();

            object apiDataset = taskOp.Result;

        }


    }
}
