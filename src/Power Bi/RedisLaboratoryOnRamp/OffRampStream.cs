

#region Usings

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using StackExchange.Redis;

#endregion

namespace SwashApp.Framework.Dcp.Redis
{
    public class OffRampStream
    {

        /// <summary>
        /// Properties
        /// </summary>

        private ISubscriber Subscriber { get; set; }

        private long MaxMessageSize { get; set; }
        private string GroupStorageAccountName { get; set; }
        private string GroupStorageAccountKey { get; set; }

        public bool CreateOffRampStream(string redisConnectionString, long maxMessageSize, string groupStorageAccountName, string groupStorageAccountKey)
        {
            try
            {
                //Properties set
                MaxMessageSize = maxMessageSize;
                GroupStorageAccountName = groupStorageAccountName;
                GroupStorageAccountKey = groupStorageAccountKey;
                //

                ConnectionMultiplexer redis =
                    ConnectionMultiplexer.Connect(redisConnectionString);

                Subscriber = redis.GetSubscriber();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                byte[] byteArray = ObjectToByteArray(message);
                if (byteArray.LongLength > MaxMessageSize)
                {
                    BlobDevicePersistentProvider blobDevicePersistentProvider = new BlobDevicePersistentProvider();
                    blobDevicePersistentProvider.PersistEventToStorage(byteArray,"123", GroupStorageAccountName,
                        GroupStorageAccountKey);
                }
                Subscriber.Publish("*", byteArray);
            }
            catch (Exception ex)
            {


            }

        }



        public static byte[] ObjectToByteArray(object objectData)
        {
            if (objectData == null)
            {
                return null;
            }

            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, objectData);
            return memoryStream.ToArray();
        }

        /// <summary>
        ///     Serialize Array to Object
        /// </summary>
        /// <param name="arrayBytes">
        ///     Array byte to deserialize
        /// </param>
        /// <returns>
        ///     Object deserialized
        /// </returns>
        public static object ByteArrayToObject(byte[] arrayBytes)
        {


            var memoryStream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();
            memoryStream.Write(arrayBytes, 0, arrayBytes.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var obj = binaryFormatter.Deserialize(memoryStream);
            return obj;
        }

    }
}