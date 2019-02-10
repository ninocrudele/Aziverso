// /* Copyright (C) Rethink121, Ltd - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * Written by Nino Crudele <nino.crudele@live.com>, 2019
//  */

#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AOC.AzureAPILibrary.HelpClasses;
using AOC.AzureAPILibrary.Log;
using AOC.AzureAPILibrary.Security;
using AOC.AzureCostsLibrary.API.Contracts;
using AOC.AzureCostsLibrary.HelpClasses;
using AOC.AzureOfficeCompanion.WorkSheet;
using Newtonsoft.Json;

#endregion

namespace AOC.AzureAPILibrary.API
{
    public static class APIHelpers
    {
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        ///     Return the number of columns
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static int GetNumberOfColumns(object typeObject, ref int columns)
        {
            var propertyInfos = typeObject.GetType().GetProperties().ToList()
                .Where(p => p.GetCustomAttributes(typeof(AzureAPIProperty), true).Length > 0);
            foreach (var property in propertyInfos)
            {
                var propertyAttributes =
                    property.GetCustomAttributes(typeof(AzureAPIProperty), true);
                var propertuAttribute = (AzureAPIProperty) propertyAttributes[0];

                if (propertuAttribute.Complex)
                    GetNumberOfColumns(property.GetValue(typeObject), ref columns);
                else
                    columns++;
            }

            return columns;
        }

        /// <summary>
        ///     Set the column names
        /// </summary>
        /// <param name="typeObject"></param>
        /// <param name="dataArray"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private static object[,] SetColumnsNameToArray(object typeObject, ref object[,] dataArray, ref int column)
        {
            var propertyInfos = typeObject.GetType().GetProperties().ToList()
                .Where(p => p.GetCustomAttributes(typeof(AzureAPIProperty), true).Length > 0);

            foreach (var property in propertyInfos)
            {
                //Get the column name
                var propertyAttributes =
                    property.GetCustomAttributes(typeof(AzureAPIProperty), true);
                var propertuAttribute = (AzureAPIProperty) propertyAttributes[0];


                if (propertuAttribute.Complex)
                {
                    SetColumnsNameToArray(property.GetValue(typeObject), ref dataArray, ref column);
                }
                else
                {
                    dataArray[0, column] = propertuAttribute.ColumnName;
                    column++;
                }
            }


            return dataArray;
        }

        /// <summary>
        /// </summary>
        /// <param name="type">The object type</param>
        /// <param name="dataArray">The array to fill</param>
        /// <param name="row">Number of row to use in the array (0 is the top for column names)</param>
        /// <returns></returns>
        private static object[,] SetColumnsValuesToArray(object typeObject, ref object[,] dataArray, ref int column,
            int row)
        {
            var propertyInfos = typeObject.GetType().GetProperties().ToList()
                .Where(p => p.GetCustomAttributes(typeof(AzureAPIProperty), true).Length > 0);
            foreach (var property in propertyInfos)
            {
                //Get the column name
                var propertyAttributes =
                    property.GetCustomAttributes(typeof(AzureAPIProperty), true);
                var propertuAttribute = (AzureAPIProperty) propertyAttributes[0];


                if (propertuAttribute.Complex)
                {
                    SetColumnsValuesToArray(property.GetValue(typeObject), ref dataArray, ref column, row);
                }
                else
                {
                    if (row == 0)
                        dataArray[row, column] = propertuAttribute.ColumnName;
                    else
                        dataArray[row, column] = property.GetValue(typeObject);
                    column++;
                }
            }

            return dataArray;
        }

        public static string GetDataString(string url, string token)
        {
            //url = "https://consumption.azure.com/v2/enrollments/51464378/pricesheet";
            //An error occurred! The service returned: StatusCode: 401, ReasonPhrase: 'Unauthorized', Version: 1.1, Content: System.Net.Http.StreamContent, Headers:
            //}Content: {"error":{"code":"401","message":"API Key is expired or invalid.|API Key is expired or invalid."}}

            HttpMethod method = null;

            //url = "https://consumption.azure.com/v1/enrollments/51464378/pricesheet";


            var methodAndUrl = url.Split(' ');

            switch (methodAndUrl[0])
            {
                case "GET":
                    method = HttpMethod.Get;
                    break;
                case "POST":
                    method = HttpMethod.Post;
                    break;
                default:
                    method = HttpMethod.Get;
                    break;
            }

            var newUrl = methodAndUrl.Length == 1 ? methodAndUrl[0] : methodAndUrl[1];
            using (var request = new HttpRequestMessage(method, newUrl))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = httpClient.SendAsync(request).Result;

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = "An error occurred! The service returned: " + response;

                    var x = response.Content.ReadAsStringAsync();
                    x.Wait();
                    errorMsg += "Content: " + x.Result;
                    throw new Exception(errorMsg);
                }

                var readTask = response.Content.ReadAsStringAsync();
                readTask.Wait();
                return readTask.Result;
            }
        }

        public static string GetAccessToken(SettingsAOC saoc)
        {
            var taskOp = AuthenticationToken.GetAccessToken(saoc.TenantId, saoc.AppId, saoc.AppKey);
            taskOp.Wait();
            var token = taskOp.Result;
            return token;
        }

        /// <summary>
        ///     Return the object required from the API call
        /// </summary>
        /// <param name="requestURL">UTL to call</param>
        /// <param name="FullyQualifiedNameOfClass">Object to use ie: AOC.AzureAPILibrary.Costs.Data.DataPayload</param>
        /// <param name="token">Auth Token</param>
        /// <returns></returns>
        public static async Task<object> GetDataObject(string requestURL, string FullyQualifiedNameOfClass,
            string token)
        {
            // Call the Usage API, dump the output to the console window
            try
            {
                //dynamic dataPayload = ObjectHelper.GetInstance("AOC.AzureAPILibrary.Resources.Data.DataPayload");
                dynamic dataPayload = ObjectHelper.GetInstance(FullyQualifiedNameOfClass);
                dynamic dataPayloadTemp = ObjectHelper.GetInstance(FullyQualifiedNameOfClass);
                var objType = Type.GetType(FullyQualifiedNameOfClass);

                //dynamic dataPayloadGlobal = ObjectHelper.GetInstance("AOC.AzureAPILibrary.Resources.Data.DataPayload");
                //  dataPayloadGlobal.value = new List<value>();

                var response = GetDataString(requestURL, token);
                var nextLink = string.Empty;
                if (response == "{\"value\":[]}") return null;

                // Convert the Stream to a strongly typed RateCardPayload object.  
                // You can also walk through this object to manipulate the individuals member objects. 

                dataPayload = JsonConvert.DeserializeObject(response, objType);

                nextLink = dataPayload.nextLink;

                while (nextLink != null)
                {
                    response = GetDataString(nextLink, token);
                    dataPayloadTemp = JsonConvert.DeserializeObject(response, objType);
                    nextLink = dataPayloadTemp.nextLink;
                    dataPayload.value.AddRange(dataPayloadTemp.value);
                }
                // Convert the Stream to a strongly typed RateCardPayload object.  
                // You can also walk through this object to manipulate the individuals member objects. 

                if (dataPayload.value.Count == 0)
                    return null;
                return dataPayload;

                //https://stackoverflow.com/questions/6428940/how-to-flatten-nested-objects-with-linq-expression
            }
            catch (Exception e)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);

                var currentMethodName = frame.GetMethod();
                LogHandling.ThrowExceptionStatement(currentMethodName.Name, e);
                return null;
            }
        }

        public static bool IsPropertyExist(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>) settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }

        public static async Task<DataSetEnlistObject> GetDataObjectFromGeneric(string requestURL, string token,
            bool withColumns, bool cleanIt, bool heuristicView, string subscriptionName, string subscriptionId,
            string filterSearch, List<EnlistDataSetValuesToQuery> dctColumnsValuesToQueries, StringBuilder sbLogs,bool showerrorMessage)
        {
            // Call the Usage API, dump the output to the console window
            try
            {
                var searchFilter = filterSearch.Length > 0;
                var stopwatch = new Stopwatch();
                var perfs = new List<string>();

                requestURL = "https://management.azure.com" + requestURL;
                var response = GetDataString(requestURL, token);

                if (response.Length == 0)
                    return null;
                var dct = JsonHelper.DeserializeAndFlatten(response);

                //provo a normalizzare un dizionario e poi riempio le righe, prima semplifico e estraggo le chiavi in dinstinct

                stopwatch.Start();

                var dctColumns = new Dictionary<string, int>();


                //Add subscription info, some API doesn't have that

                var orderItem = 0;


                //Create columns
                foreach (var kvp in dct)
                {
                    var i = kvp.Key.LastIndexOf(".");
                    //string key = (i > -1 ? kvp.Key.Substring(i + 1) : kvp.Key);
                    var m = Regex.Match(kvp.Key, @"\.([0-9]+)\.");
                    var key = i > -1 ? kvp.Key.Substring(m.Index + m.Length) : kvp.Key;
                    if (m.Success)
                        try
                        {
                            if (cleanIt)
                            {
                                //MatchCollection guids = Regex.Matches(findGuid, @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}"); //Match all substrings in findGuid
                                var guids = Regex.Match(kvp.Value.ToString(),
                                    @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}"); //Match all substrings in findGuid

                                if (guids.Success == false && kvp.Value.ToString().IndexOf("/subscriptions/") < 0)
                                    if (!dctColumns.ContainsKey(key))
                                    {
                                        dctColumns.Add(key, orderItem);
                                        orderItem++;
                                    }

                                if (key.ToLower() == "id")
                                    if (!dctColumns.ContainsKey(key))
                                    {
                                        dctColumns.Add(key, orderItem);
                                        orderItem++;
                                    }
                            }
                            else
                            {
                                dctColumns.Add(key, orderItem);
                                orderItem++;
                            }
                        }
                        catch
                        {
                        }
                }

                //Change03122018
                dctColumns.Add("subscriptionName", orderItem++);
                dctColumns.Add("subscriptionId", orderItem++);
                dctColumns.Add("Filter", orderItem++);
                stopwatch.Stop();
                perfs.Add($"Dernomalization: {stopwatch.Elapsed}");
                stopwatch.Start();

                //dctNormalised per trovatre indice
                var dtsArray = new object[1000, dctColumns.Count];
                //dtsArray[1, 0] = dct.First().Value;
                var breakKey = dctColumns.First().Key;
                var rowNumber = 1;
                var colNumber = 0;


                if (withColumns)
                    foreach (var item in dctColumns)
                        dtsArray[0, item.Value] = item.Key;
                else
                    rowNumber = 0;

                stopwatch.Stop();
                perfs.Add($"Set columns: {stopwatch.Elapsed}");
                stopwatch.Start();


                var firstTime = false;
                firstTime = true;
                //Adesso ho i campi e per ogni campo estraggo il nome e setto il valore
                foreach (var kvp in dct)
                {
                    var i = kvp.Key.LastIndexOf(".");
                    //string key = (i > -1 ? kvp.Key.Substring(i + 1) : kvp.Key);
                    var m = Regex.Match(kvp.Key, @"\.([0-9]+)\.");
                    var key = i > -1 ? kvp.Key.Substring(m.Index + m.Length) : kvp.Key;
                    if (m.Success)
                        try
                        {
                            if (breakKey == key)
                            {
                                //Here is a new array row , i add the subscription info
                                //dtsArray[rowNumber, dctColumns.Count - 3] = subscriptionName;
                                //dtsArray[rowNumber, dctColumns.Count - 2] = subscriptionId;
                                dctColumnsValuesToQueries.Add(new EnlistDataSetValuesToQuery("SubsctiptionName",
                                    string.Concat("SubsctiptionName - [", subscriptionName, "]")));
                                dctColumnsValuesToQueries.Add(new EnlistDataSetValuesToQuery("SubsctiptionID",
                                    string.Concat("SubsctiptionID - [", subscriptionId, "]")));
                                if (firstTime)
                                    firstTime = false;
                                else
                                    rowNumber++;
                            }


                            if (dctColumns.ContainsKey(key))
                            {
                                colNumber = dctColumns[key];
                                dtsArray[rowNumber, colNumber] = kvp.Value;
                                dtsArray[rowNumber, dctColumns.Count - 3] = subscriptionName;
                                dtsArray[rowNumber, dctColumns.Count - 2] = subscriptionId;
                                dctColumnsValuesToQueries.Add(new EnlistDataSetValuesToQuery(key,
                                    string.Concat(key, " - [", kvp.Value.ToString(), "]")));

                                if (searchFilter)
                                    if (kvp.Value.ToString().IndexOf(filterSearch) >= 0)
                                        dtsArray[rowNumber, dctColumns.Count - 1] =
                                            kvp.Value.ToString().IndexOf(filterSearch) >= 0 ? "X" : string.Empty;
                            }
                        }
                        catch
                        {
                        }
                }

                stopwatch.Stop();
                perfs.Add($"Set values: {stopwatch.Elapsed}");
                stopwatch.Start();

                //object[,]newdtsArray = ArrayHelper.TrimArray(rowNumber + 1, 10000, dtsArray);

                var wordsOccurences = new Dictionary<string, int>();
                var wordsOccurencesCleaned = new Dictionary<string, int>();
                try
                {
                    if (heuristicView)
                    {
                        for (var iRow = 1; iRow < dtsArray.GetLength(0); iRow++)
                        for (var iCol = 0; iCol < dtsArray.GetLength(1); iCol++)
                        {
                            var itemIncrement = 0;
                            if (dtsArray[iRow, iCol] != null)
                            {
                                var value = dtsArray[iRow, iCol].ToString();
                                if (value != null)
                                {
                                    if (wordsOccurences.ContainsKey(value))
                                    {
                                        itemIncrement = wordsOccurences[value];
                                        wordsOccurences[value] = itemIncrement + 1;
                                    }
                                    else
                                    {
                                        wordsOccurences.Add(value, itemIncrement);
                                    }
                                }
                            }
                        }

                        stopwatch.Stop();
                        perfs.Add($"Heuristic: {stopwatch.Elapsed}");
                        stopwatch.Start();

                        //Clean the words list from the 0

                        foreach (var item in wordsOccurences)
                            if (item.Value > 0)
                                wordsOccurencesCleaned.Add(item.Key, item.Value);

                        stopwatch.Stop();
                        perfs.Add($"Cleaning: {stopwatch.Elapsed}");
                    }
                }
                catch
                {
                }


                //groups


                var dataSetEnlistObject = new DataSetEnlistObject(dtsArray, wordsOccurencesCleaned);

                return dataSetEnlistObject;
            }
            catch (Exception e)
            {
                var stackTrace = new StackTrace();
                var frame = stackTrace.GetFrame(0);

                var currentMethodName = frame.GetMethod();
                sbLogs.AppendLine("-------------------------------------------------------------------------");
                sbLogs.AppendLine($"subscriptionId {subscriptionId}\r{e.Message}\r{e.StackTrace}");
                if (showerrorMessage)
                {
                    LogHandling.ThrowExceptionStatement(currentMethodName.Name, e);

                }
                return null;
            }
        }

        public static object[,] Create2DArrayWithValueNode(dynamic dataPayload)
        {
            //Create 2d Array
            //I prefer to use a simple procedural approach instead of lopping etc,
            //this give me more granular and simple control on each field
            //+1 because the first column
            var cols = 0;
            int columns = GetNumberOfColumns(dataPayload.value[0], ref cols);
            int rows = dataPayload.value.Count;
            var retArray = new object[rows + 1, columns];

            var column = 0;

            SetColumnsNameToArray(dataPayload.value[0], ref retArray, ref column);

            var row = 1;
            column = 0;

            foreach (var rowData in dataPayload.value)
            {
                SetColumnsValuesToArray(rowData, ref retArray, ref column, row);
                column = 0;
                row++;
            }

            return retArray;
        }

        public static object[,] Create2DArray(dynamic dataPayload)
        {
            //Create 2d Array
            //I prefer to use a simple procedural approach instead of lopping etc,
            //this give me more granular and simple control on each field
            //+1 because the first column
            var cols = 0;
            int columns = GetNumberOfColumns(dataPayload, ref cols);
            int rows = dataPayload.value.Count;
            var retArray = new object[rows + 1, columns];

            var column = 0;

            SetColumnsNameToArray(dataPayload, ref retArray, ref column);

            var row = 1;
            column = 0;

            foreach (var rowData in dataPayload.value)
            {
                SetColumnsValuesToArray(rowData, ref retArray, ref column, row);
                column = 0;
                row++;
            }

            return retArray;
        }
    }
}