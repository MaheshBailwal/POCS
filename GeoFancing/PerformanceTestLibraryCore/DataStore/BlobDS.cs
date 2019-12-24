using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTestLibrary
{
    public class BlobDS : INonQueryableDataStore
    {
        private readonly string _containerName;
        private readonly string _storageconnectionstring; 

        public BlobDS(string containerName, string storageconnectionstring)
        {
            _containerName = containerName;
            _storageconnectionstring = storageconnectionstring;           
        }

        public T Get<T>(string key, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var cacheEntry = parse(DownloadFileFromBlob(key));
            stopwatch.Stop();
            fetchTime = stopwatch.Elapsed.TotalMilliseconds;
            return JsonConvert.DeserializeObject<T>(cacheEntry);
        }

        public void Put<T>(string key, T instance)
        {
            var json = JsonConvert.SerializeObject(instance);
            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            string fileName = String.Format("{0}/{1}.json", "Site", "Site" + key);
            UploadFileToBlob(fileName, byteArray, "");
        }
        
        private string UploadFileToBlob(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {

                var _task = Task.Run(() => this.UploadFileToBlobAsync(strFileName, fileData, fileMimeType));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }    
        

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageconnectionstring);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = _containerName;
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                string fileName = strFileName;

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (fileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    //cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        private byte[] DownloadFileFromBlob(string key)
        {
            var storageAccount = CloudStorageAccount.Parse(_storageconnectionstring);
            var blobClient = storageAccount.CreateCloudBlobClient();
            // Get Blob Container  
            var container = blobClient.GetContainerReference(_containerName);
            // Get reference to blob (binary content)  
            var blockBlob = container.GetBlockBlobReference("Site/Site" + key + ".json");
            // Read content  
            using (var ms = new MemoryStream())
            {
                if (blockBlob.ExistsAsync().Wait(2000))
                {
                    blockBlob.DownloadToStreamAsync(ms).Wait();
                }
                return ms.ToArray();
            }
        }

        public static string parse(byte[] json)
        {
            string jsonStr = Encoding.UTF8.GetString(json);
            return jsonStr;
        }
    }
}

