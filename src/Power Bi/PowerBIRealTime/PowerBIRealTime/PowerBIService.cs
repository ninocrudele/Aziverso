using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PowerBIRealTime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace PowerBIRealTime
{
    public class PowerBIService
    {
        private readonly string authorityUrl;
        private readonly string resourceUrl;
        private readonly string appId;
        private readonly string appSecret;
        private readonly Uri redirectUri;
        private Uri baseAddress = new Uri("https://api.powerbi.com/v1.0/myorg");

        public PowerBIService()
        {
            authorityUrl = ConfigurationManager.AppSettings["powerbi:AuthorityURI"];
            resourceUrl = ConfigurationManager.AppSettings["powerbi:ResourceURI"];
            appId = ConfigurationManager.AppSettings["powerbi:AppId"];
            redirectUri = new Uri(ConfigurationManager.AppSettings["powerbi:RedirectUri"]);
            appSecret = ConfigurationManager.AppSettings["powerbi:AppSecret"];
        }

        public async Task<string> GetAccessToken()
        {
            var authContext = new AuthenticationContext(authorityUrl);

            // Get the Access Token using the Authorization Code sent by Azure AD

            var creds = new ClientCredential(appId, appSecret);


            //var result = await authContext.AcquireTokenAsync(resourceUrl
            //    , appId
            //    , redirectUri
            //    , new PlatformParameters(PromptBehavior.Auto));

            var result = await authContext.AcquireTokenAsync("https://analysis.windows.net/powerbi/api"
                , appId
                , new UserPasswordCredential("nino.crudele@rethink121.com", "T3st3d1k@y@k"));



            var accessToken = result.AccessToken;

            return accessToken;
        }

        public async Task<List<PBIDataSet>> GetDataSets(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException("authToken");
            
            var datasets = new List<PBIDataSet>();            

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", authToken));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                
                using (var response = await client.GetAsync(String.Format("{0}/datasets", baseAddress)))
                {
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();                                       

                    var oResponse = JObject.Parse(responseString);

                    datasets = oResponse.SelectToken("value").ToObject<List<PBIDataSet>>();
                }
            }

            return datasets;
        }

        public async Task<List<PBIDashboard>> GetDashboards(string authToken)
        {
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException("authToken");

            var dashboards = new List<PBIDashboard>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", authToken));
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                using (var response = await client.GetAsync(String.Format("{0}/dashboards", baseAddress)))
                {
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();

                    var oResponse = JObject.Parse(responseString);

                    dashboards = oResponse.SelectToken("value").ToObject<List<PBIDashboard>>();
                }
            }

            return dashboards;
        }

        public async Task<List<PBIDashboardTile>> GetDashboardTiles(string authToken, string dashboardId)
        {
            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException("authToken");
            if (string.IsNullOrEmpty(dashboardId))
                throw new ArgumentNullException("dashboardId");

            var dashboards = new List<PBIDashboardTile>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", authToken));
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                using (var response = await client.GetAsync(String.Format("{0}/dashboards/{1}/tiles", baseAddress, dashboardId)))
                {
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();

                    var oResponse = JObject.Parse(responseString);

                    dashboards = oResponse.SelectToken("value").ToObject<List<PBIDashboardTile>>();
                }
            }

            return dashboards;
        }

        public async Task<PBIDataSet> CreateDataSet(string authToken, PBIDataSet dataSet, bool includeData = false)
        {
            if (authToken == null)
                throw new ArgumentNullException("authToken");
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", authToken));
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var jObj = JObject.FromObject(dataSet, GetJSONSerializer());
                
                jObj.Remove("id");
                jObj.Remove("defaultRetentionPolicy");

                var jObjText = jObj.ToString();

                var bodyContent = new StringContent(jObjText, System.Text.Encoding.Default, "application/json");
 
                using (var response = await client.PostAsync(String.Format("{0}/datasets", baseAddress), bodyContent))
                {                    
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();

                    var oResponse = JObject.Parse(responseString);

                    dataSet.Id = oResponse.SelectToken("id").ToString();
                }
            }

            if (includeData)
            {
                foreach (var table in dataSet.Tables)
                {
                    await AddTableRows(authToken, dataSet.Id, table.Name, table.Rows, 500);
                }
            }

            return dataSet;
        }

        public async Task AddTableRows(string authToken, string dataSetId, string tableName, IEnumerable<dynamic> rows, int batchSize = 1000)
        {
            if (string.IsNullOrEmpty(dataSetId))
                throw new ArgumentNullException("dataSetId");
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");
            if (rows == null)
                throw new ArgumentNullException("rows");
                        
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", authToken));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                
                foreach (var rowsBatch in Batch(rows, batchSize))
                {
                    var jObj = new JObject();
                    jObj.Add("rows", JToken.FromObject(rowsBatch));

                    var jObjText = jObj.ToString();

                    var bodyContent = new StringContent(jObjText, System.Text.Encoding.Default, "application/json");

                    using (var response = await client.PostAsync(String.Format("{0}/datasets/{1}/tables/{2}/rows", baseAddress, dataSetId, tableName), bodyContent))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }            
        }

        private static JsonSerializer GetJSONSerializer()
        {
            return JsonSerializer.Create(new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public async Task ClearTable(string authToken, string dataSetId, string tableName)
        {
            if (string.IsNullOrEmpty(dataSetId))
                throw new ArgumentNullException("dataSetId");
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("tableName");

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Bearer {0}", authToken));
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                using (var response = await client.DeleteAsync(String.Format("{0}/datasets/{1}/tables/{2}/rows", baseAddress, dataSetId, tableName)))
                {
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();
                }
            }
        }

        public IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> collection, int batchSize)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            List<T> nextbatch = new List<T>(batchSize);

            foreach (T item in collection)
            {
                nextbatch.Add(item);
                if (nextbatch.Count == batchSize)
                {
                    yield return nextbatch;
                    nextbatch = new List<T>(batchSize);
                }
            }

            if (nextbatch.Count > 0)
            {
                yield return nextbatch;
            }
        }

        public static string GenerateSignInUrl(string replyUrl)
        {
            return new PowerBIService().GenerateSignInUrlInternal(replyUrl);
        }

        private string GenerateSignInUrlInternal(string replyUrl)
        {
            if (string.IsNullOrEmpty(replyUrl))
                throw new ArgumentNullException("replyUrl");

            return string.Format("{0}?resource={1}&client_id={2}&response_type=code&redirect_uri={3}&prompt=login"
            //return string.Format("{0}?resource={1}&client_id={2}&response_type=code&redirect_uri={3}"
                , authorityUrl
                , System.Web.HttpUtility.UrlEncode(resourceUrl)
                , appId
                , System.Web.HttpUtility.UrlEncode(replyUrl)
                );
        }
    }
}