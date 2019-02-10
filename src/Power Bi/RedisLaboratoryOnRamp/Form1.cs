using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure;
using Microsoft.Azure.Management.Redis;
using Microsoft.Azure.Management.Redis.Models;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Management.Models;
using Microsoft.WindowsAzure.Management.Storage;
using Microsoft.WindowsAzure.Management.Storage.Models;
using Microsoft.WindowsAzure.Management.Compute;
using Microsoft.WindowsAzure.Management.Compute.Models;

using Newtonsoft.Json;
using SwashApp.Framework.Dcp.Redis;
using CertificateCloudCredentials = Microsoft.WindowsAzure.CertificateCloudCredentials;
using TokenCloudCredentials = Microsoft.WindowsAzure.TokenCloudCredentials;

namespace RedisLaboratoryOnRamp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonrun_Click(object sender, EventArgs e)
        {

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            OffRampStream offRampStream = new OffRampStream();
            offRampStream.CreateOffRampStream("localhost:6379", 500000000, "swashapp",
                "kgZU7tipZJoEWqlmJeA9gDgT8A32l9qzsWz5an1JzY4WRX4UdJs2LEKE/DMPGWKPXx/VTRR2nYHzgTB4O+D/2g==");
            offRampStream.SendMessage(textBoxMessage.Text);

        }
        /// <summary>
        /// Main properties
        /// </summary>

        private bool IntegrationStackEnabled { get; set; }

        private void buttonPrepare_Click(object sender, EventArgs e)
        {
            while (true)
            {
                string jsonToken = "{\"subscriptionId\":\"a007ae31-430b-456f-a55b-37b45e66fd3d\"," +
                   "\"certificateissuer\":\"swashgate\"," +
                   "\"tenantid\":\"e49dcee2-ac46-4965-9a43-df7c06959a00\"," +
                   "\"clientid\":\"a72a11c8-c59f-491b-90b0-958b3ebcad5a\"," +
                   "\"key\":\"EAI8jrpbkjluVhiKVIdGqXsp9wPNn9Hwx1CcNIGp78s=\"," +
                   "\"resourcegroupname\":\"grabcaster\"," +
                   "\"storagegroupname\":\"redistestnino4\"}";

                if (CreateintegrationStack(jsonToken))
                {
                    
                }
                else
                {
                    
                }


            }


        }

        /// <summary>
        /// Create the integration stack 
        /// </summary>
        /// <param name="jsonToken"></param>
        /// <returns></returns>
        private bool CreateintegrationStack(string jsonToken)
        {
            try
            {
                Dictionary<string, string> jsonSecurityToken = new Dictionary<string, string>();
                //Create json security token
                jsonSecurityToken = JsonTokenObjectCreate(jsonToken);
                string subscriptionId = jsonSecurityToken["subscriptionId"];

                //s.Redis.Create()

                ////Certificate for credential
                //X509Certificate2 x509Certificate2 = GetStoreCertificate(jsonSecurityToken["certificateissuer"]);
                //CertificateCloudCredentials credentials = new CertificateCloudCredentials(subscriptionId, x509Certificate2);

                CreateStorageAccount(jsonSecurityToken);

                CreateRedisCache(jsonSecurityToken);

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }
        }

        private static TokenCredentials CreateTokenCredential(Dictionary<string, string> jsonSecurityToken)
        {

            string token = GetAuthorizationHeader(jsonSecurityToken["tenantid"],
                jsonSecurityToken["clientid"], jsonSecurityToken["key"]);

            TokenCredentials serviceClientCredentials = new TokenCredentials(token);
            return serviceClientCredentials;

        }
        private static void CreateRedisCache(Dictionary<string, string> jsonSecurityToken)
        {

            TokenCredentials serviceClientCredentials = CreateTokenCredential(jsonSecurityToken);
            Microsoft.Azure.Management.Redis.RedisManagementClient redisClient =
                new RedisManagementClient(serviceClientCredentials);

            redisClient.SubscriptionId = jsonSecurityToken["subscriptionId"];
            var redisResource = redisClient.Redis.Create(jsonSecurityToken["resourcegroupname"],
                jsonSecurityToken["storagegroupname"], new RedisCreateParameters()
                {
                    EnableNonSslPort = false,
                    Location = LocationNames.NorthEurope,
                    Sku = new Sku()
                    {
                        Family = SkuFamily.C,
                        Name = SkuName.Basic
                    }
                }
            );
        }
        private void CreateStorageAccount(Dictionary<string, string> jsonSecurityToken)
        {
            TokenCredentials tokenCredentials = CreateTokenCredential(jsonSecurityToken);

            var resourceManagementClient = new ResourceManagementClient(tokenCredentials)
            {
                
                SubscriptionId = jsonSecurityToken["subscriptionId"]
        };

            // Add a Microsoft.Storage provider to the subscription.
            var storageProvider = resourceManagementClient.Providers.Register("Microsoft.Storage");

            string token = GetAuthorizationHeader(jsonSecurityToken["tenantid"],
                jsonSecurityToken["clientid"], jsonSecurityToken["key"]);
            // Auth..

            var storageManagementClient =
                new StorageManagementClient(
                    new Microsoft.Azure.TokenCloudCredentials(jsonSecurityToken["subscriptionId"], token));
            // Your new awesome storage account.
            var response = storageManagementClient.StorageAccounts.Create(new StorageAccountCreateParameters()
            {
                Location = LocationNames.NorthEurope,
                Name = "ninostorage22",
                Description = "storage from code",
                AccountType = StorageAccountTypes.StandardGRS
            });
            

         
        }
        /// <summary>
        /// Create the storage account
        /// </summary>
        /// <param name="credentials"></param>
        private static void CreateStorageAccountOLD(CertificateCloudCredentials credentials)
        {
            //var storageClient = CloudContext.Clients.CreateStorageManagementClient(credentials);

            //var response = storageClient.StorageAccounts.Create(new StorageAccountCreateParameters()
            //{
            //    Location = LocationNames.NorthEurope,
            //    Name = "ninostorage2",
            //    Description = "storage from code",
            //    AccountType = StorageAccountTypes.StandardGRS
            //});

            //Console.WriteLine(response.StatusCode);
        }
        /// <summary>
        /// Get the certificate
        /// </summary>
        /// <param name="issuer"></param>
        /// <returns></returns>
        public X509Certificate2 GetStoreCertificate(string issuer)
        {
            List<StoreLocation> locations = new List<StoreLocation>
            {
                StoreLocation.CurrentUser,
                StoreLocation.LocalMachine
            };

            foreach (var location in locations)
            {
                X509Store store = new X509Store("My", location);
                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
                    X509Certificate2Collection certificates = store.Certificates.Find(X509FindType.FindByIssuerName,
                        issuer, false);

                    if (certificates.Count == 1)
                    {
                        return certificates[0];
                    }
                }
                finally
                {
                    store.Close();
                }
            }

            throw new ApplicationException("No Certificate found");
        }
        /// <summary>
        /// Create the authorization header
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="clientId"></param> //Application ID
        /// <param name="clientSecrets"></param>
        /// <returns></returns>
        private static string GetAuthorizationHeader(string tenantId, string clientId, string clientSecrets)
        {
            //https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-create-service-principal-portal

            AuthenticationResult result = null;

            var context = new AuthenticationContext(String.Format("https://login.windows.net/{0}", tenantId));

            var thread = new Thread(() =>
            {
                var authParam = new PlatformParameters(PromptBehavior.Never, null);
                result = context.AcquireTokenAsync(
                    "https://management.core.windows.net/",
                    new ClientCredential(clientId, clientSecrets)).Result;
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Name = "AquireTokenThread";
            thread.Start();
            thread.Join();


            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            string token = result.AccessToken;
            return token;
        }

        /// <summary>
        /// Create the json token object
        /// </summary>
        /// <param name="jsonToken"></param>
        /// <returns></returns>
        private Dictionary<string, string> JsonTokenObjectCreate(string jsonToken)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonToken);

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        /// <summary>
        /// Serialize the json object token into string
        /// </summary>
        /// <param name="jsonToken"></param>
        /// <returns></returns>
        private string JsonTokenObjectSerialize(Dictionary<string, string> jsonToken)
        {
            try
            {
                return JsonConvert.SerializeObject(jsonToken);

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void buttonJson_Click(object sender, EventArgs e)
        {
            

            //jsonSecurityToken.Add("d1", "d1");
            //jsonSecurityToken.Add("d2", "d2");
            //jsonSecurityToken.Add("d3", "d3");
            //jsonSecurityToken.Add("d4", "d4");



           // string jsonToken = JsonConvert.SerializeObject(jsonSecurityToken);

            //Dictionary<string, string> jsonSecurityToken2 = new Dictionary<string, string>();

            //jsonSecurityToken2 =JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonToken);
        }

    }
}
