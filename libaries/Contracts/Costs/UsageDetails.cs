using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AOC.AzureAPILibrary.Costs.Data;
using Newtonsoft.Json;
using AOC.AzureAPILibrary.HelpClasses;
using AOC.AzureAPILibrary.Log;
using AOC.AzureAPILibrary.Security;
using AOC.AzureAPILibrary.Web;

namespace AOC.AzureAPILibrary.API.Costs
{
    public class UsageDetails
    {
        //Call
        //GET https://management.azure.com/subscriptions/00000000-0000-0000-0000-000000000000/providers/Microsoft.Billing/billingPeriods/201810/providers/Microsoft.Consumption/usageDetails?api-version=2018-10-01


        /// <summary>
        /// Public properties
        /// </summary>
        private string subscriptionID {get;set;}

        public UsageDetails(string SubscriptionID)
        {
            subscriptionID = SubscriptionID;

        }
        /// <summary>
        /// https://docs.microsoft.com/en-us/rest/api/consumption/usagedetails/listbybillingperiod
        /// </summary>
        /// <returns></returns>
        public async Task<object[,]> GetData()
        {
            string requestURL = $"https://management.azure.com/subscriptions/{subscriptionID}/providers/Microsoft.Billing/billingPeriods/201810/providers/Microsoft.Consumption/usageDetails?api-version=2018-10-01&top=1";
            //string requestURL = $"https://management.azure.com/subscriptions?api-version=2016-06-01";
            
            var taskOp = AuthenticationToken.GetAccessToken("Hexmet.onmicrosoft.com", "53152e2a-7dba-4776-af97-b597cf8fa112", "w;/:L&-^]:.v>}*{}#(}_%&/.:0*4I(+.$#;)");
            taskOp.Wait();
            string token = taskOp.Result;

            HttpWebRequest request = (HttpWebRequest)HTTPClientAPI.Create(requestURL, token);

            // Call the Usage API, dump the output to the console window
            try
            {
                DataPayload dataPayload = new DataPayload();
                DataPayload dataPayloadGlobal = new DataPayload();
                dataPayloadGlobal.value = new List<value>();
                dataPayload.nextLink = String.Empty;
                while (dataPayload.nextLink != null)
                {


                    // Call the REST endpoint
                    Console.WriteLine("Calling Usage service...");
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Console.WriteLine(String.Format("Usage service response status: {0}", response.StatusDescription));
                    Stream receiveStream = response.GetResponseStream();


                    using (var fileStream = new FileStream(@"C:\Users\Nino\Desktop\azurersubscription.json", FileMode.Create, FileAccess.Write))
                    {
                        receiveStream.CopyTo(fileStream);
                    }

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    var usageResponse = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();

                    // Convert the Stream to a strongly typed RateCardPayload object.  
                    // You can also walk through this object to manipulate the individuals member objects. 
                    dataPayload = JsonConvert.DeserializeObject<DataPayload>(usageResponse);
                    dataPayloadGlobal.value.AddRange(dataPayload.value);
                    if (dataPayload.nextLink != null)
                    {
                        request = (HttpWebRequest)HTTPClientAPI.Create(dataPayload.nextLink, token);
                    }

                }

                //                //Copy the infofields in the propeties to optimize the conversion
                //                DataPayload newDataPayload = new DataPayload();
                //                newDataPayload.value = new List<value>();
                var retFlatObject = 
                    from v in dataPayloadGlobal.value 
                    select new
                    {
                        id = v.id,
                        name = v.name,
                        tags = v.tags,
                        type = v.type,
                        billingPeriodId = v.properties.billingPeriodId, // ": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/providers/Microsoft.Billing/billingPeriods/20181001",
                        usageStart = v.properties.usageStart, //": "2018-10-22T00:00:00.0000000Z",
                        usageEnd = v.properties.usageEnd, //": "2018-10-22T23:59:59.0000000Z",
                        instanceId = v.properties.instanceId, //": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/resourceGroups/MI-BI-RG/providers/Microsoft.Storage/storageAccounts/mibirgdiag592",
                        instanceName = v.properties.instanceName, //": "mibirgdiag592",
                        instanceLocation = v.properties.instanceLocation, //": "euwest",
                        meterId = v.properties.meterId, //": "923978e1-fd3f-4bd5-a798-f4b533057e46",
                        usageQuantity = v.properties.usageQuantity, //": 0.2879,
                        pretaxCost = v.properties.pretaxCost, //": 0.000074456896552,
                        currency = v.properties.currency, //": "EUR",
                        isEstimated = v.properties.isEstimated, //": false,
                        subscriptionGuid = v.properties.subscriptionGuid, //": "12ba10cc-a5b2-4bc7-a5ae-615729c05167",
                        subscriptionName = v.properties.subscriptionName, //": "Hexagon MI BI",
                        accountName = v.properties.accountName, //": "HEXAGON MI",
                        departmentName = v.properties.departmentName, //": "MI-IT",
                        product = v.properties.product, //": "General Block Blob - Delete Operations",
                        consumedService = v.properties.consumedService, //": "Microsoft.Storage",
                        costCenter = v.properties.costCenter, //": "Hexagon MI",
                        partNumber = v.properties.partNumber, //": "N9H-00741",
                        resourceGuid = v.properties.resourceGuid, //": "923978e1-fd3f-4bd5-a798-f4b533057e46",
                        offerId = v.properties.offerId, //": "MS-AZR-0017P",
                        chargesBilledSeparately = v.properties.chargesBilledSeparately, //": false,
                        location = v.properties.location, //": "Unassigned",
                        meterDetails = v.properties.meterDetails //": null
                        };

                //Create 2d Array
                //I prefer to use a simple procedural approach instead of lopping etc,
                //this give me more granular and simple control on each field
                //+1 because the first column
                object[,] dataArray = new object[retFlatObject.Count() + 1, 28];
                
                ////Excel Columns
                dataArray[0, 0] = "departmentName"; //": "MI-IT",
                dataArray[0, 1] = "accountName"; //": "HEXAGON MI",
                dataArray[0, 2] = "subscriptionName"; //": "Hexagon MI BI",
                dataArray[0, 3] = "costCenter"; //": "Hexagon MI",
                dataArray[0, 4] = "product"; //": "General Block Blob - Delete Operations",
                dataArray[0, 5] = "consumedService"; //": "Microsoft.Storage",
                dataArray[0, 6] = "partNumber"; //": "N9H-00741",
                dataArray[0, 7] = "instanceName"; //": "mibirgdiag592",
                dataArray[0, 8] = "instanceLocation"; //": "euwest",
                dataArray[0, 9] = "usageQuantity"; //": 0.2879,
                dataArray[0, 10] = "pretaxCost"; //": 0.000074456896552,
                dataArray[0, 11] = "currency"; //": "EUR",
                dataArray[0, 12] = "usageStart"; //": "2018-10-22T00:00:00.0000000Z",
                dataArray[0, 13] = "usageEnd"; //": "2018-10-22T23:59:59.0000000Z",
                dataArray[0, 14] = "tags";
                dataArray[0, 15] = "offerId"; //": "MS-AZR-0017P",
                dataArray[0, 16] = "subscriptionGuid"; //": "12ba10cc-a5b2-4bc7-a5ae-615729c05167",
                dataArray[0, 17] = "billingPeriodId"; // ": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/providers/Microsoft.Billing/billingPeriods/20181001",
                dataArray[0, 18] = "instanceId"; //": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/resourceGroups/MI-BI-RG/providers/Microsoft.Storage/storageAccounts/mibirgdiag592",
                dataArray[0, 19] = "meterId"; //": "923978e1-fd3f-4bd5-a798-f4b533057e46",
                dataArray[0, 20] = "isEstimated"; //": false,
                dataArray[0, 21] = "resourceGuid"; //": "923978e1-fd3f-4bd5-a798-f4b533057e46",
                dataArray[0, 22] = "chargesBilledSeparately"; //": false,
                dataArray[0, 23] = "location"; //": "Unassigned",
                dataArray[0, 24] = "meterDetails"; //": null
                dataArray[0, 25] = "id";
                dataArray[0, 27] = "name";
                dataArray[0, 27] = "type";

                int row = 1;
                foreach (var rowData in retFlatObject)
                {
                    dataArray[row, 0] = rowData.departmentName; //": "MI-IT",
                    dataArray[row, 1] = rowData.accountName; //": "HEXAGON MI",
                    dataArray[row, 2] = rowData.subscriptionName; //": "Hexagon MI BI",
                    dataArray[row, 3] = rowData.costCenter; //": "Hexagon MI",
                    dataArray[row, 4] = rowData.product; //": "General Block Blob - Delete Operations",
                    dataArray[row, 5] = rowData.consumedService; //": "Microsoft.Storage",
                    dataArray[row, 6] = rowData.partNumber; //": "N9H-00741",
                    dataArray[row, 7] = rowData.instanceName; //": "mibirgdiag592",
                    dataArray[row, 8] = rowData.instanceLocation; //": "euwest",
                    dataArray[row, 9] = rowData.usageQuantity; //": 0.2879,
                    dataArray[row, 10] = rowData.pretaxCost; //": 0.000074456896552,
                    dataArray[row, 11] = rowData.currency; //": "EUR",
                    dataArray[row, 12] = rowData.usageStart; //": "2018-10-22T00:00:00.0000000Z",
                    dataArray[row, 13] = rowData.usageEnd; //": "2018-10-22T23:59:59.0000000Z",
                    dataArray[row, 14] = rowData.tags;
                    dataArray[row, 15] = rowData.offerId; //": "MS-AZR-0017P",
                    dataArray[row, 16] = rowData.subscriptionGuid; //": "12ba10cc-a5b2-4bc7-a5ae-615729c05167",
                    dataArray[row, 17] = rowData.billingPeriodId; // ": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/providers/Microsoft.Billing/billingPeriods/20181001",
                    dataArray[row, 18] = rowData.instanceId; //": "/subscriptions/12ba10cc-a5b2-4bc7-a5ae-615729c05167/resourceGroups/MI-BI-RG/providers/Microsoft.Storage/storageAccounts/mibirgdiag592",
                    dataArray[row, 19] = rowData.meterId; //": "923978e1-fd3f-4bd5-a798-f4b533057e46",
                    dataArray[row, 20] = rowData.isEstimated; //": false,
                    dataArray[row, 21] = rowData.resourceGuid; //": "923978e1-fd3f-4bd5-a798-f4b533057e46",
                    dataArray[row, 22] = rowData.chargesBilledSeparately; //": false,
                    dataArray[row, 23] = rowData.location; //": "Unassigned",
                    dataArray[row, 24] = rowData.meterDetails; //": null
                    dataArray[row, 25] = rowData.id;
                    dataArray[row, 27] = rowData.name;
                    dataArray[row, 27] = rowData.type;
                    row++;
                }

                //https://stackoverflow.com/questions/6428940/how-to-flatten-nested-objects-with-linq-expression
                return dataArray;

            }
            catch (Exception e)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);

                var currentMethodName = frame.GetMethod();
                LogHandling.ThrowExceptionStatement(currentMethodName.Name,e);
                throw;
            }
        }

    }
}
