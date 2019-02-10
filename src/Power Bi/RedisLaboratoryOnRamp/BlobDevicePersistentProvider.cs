
#region Usings

using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

#endregion

namespace SwashApp.Framework.Dcp.Redis
{
    /// <summary>
    ///     Main persistent provider.
    /// </summary>
    public class BlobDevicePersistentProvider
    {
        public void PersistEventToStorage(byte[] messageBody, string messageId, string GroupStorageAccountName, string GroupStorageAccountKey)
        {
            try
            {
                var storageAccountName = GroupStorageAccountName;
                var storageAccountKey = GroupStorageAccountKey;
                var connectionString =
                    $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container. 
                var container = blobClient.GetContainerReference(GroupStorageAccountName + "storage");

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(
                    new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

                // Create the messageid reference
                var blockBlob = container.GetBlockBlobReference(messageId);
                blockBlob.UploadFromByteArray(messageBody, 0, messageBody.Length);
                Debug.WriteLine(
                    "Event persisted -  Consistency Transaction Point created.");
            }
            catch (Exception ex)
            {

            }
        }

        public byte[] PersistEventFromStorage(string messageId,string GroupStorageAccountName,string GroupStorageAccountKey)
        {
            try
            {
                var storageAccountName = GroupStorageAccountName;
                var storageAccountKey = GroupStorageAccountKey;
                var connectionString =
                    $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve a reference to a container. 
                var container = blobClient.GetContainerReference(GroupStorageAccountName + "storage");

                // Create the container if it doesn't already exist.
                container.CreateIfNotExists();
                container.SetPermissions(
                    new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

                // Create the messageid reference
                var blockBlob = container.GetBlockBlobReference(messageId);

                blockBlob.FetchAttributes();
                var msgByteLength = blockBlob.Properties.Length;
                var msgContent = new byte[msgByteLength];
                for (var i = 0; i < msgByteLength; i++)
                {
                    msgContent[i] = 0x20;
                }

                blockBlob.DownloadToByteArray(msgContent, 0);

                Debug.WriteLine(
                    "Event persisted recovered -  Consistency Transaction Point restored.");

                return msgContent;
            }
            catch (Exception ex)
            {

                return null;
            }
        }
    }
}