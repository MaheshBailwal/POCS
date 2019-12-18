using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RedisTest
{
    public class CosmoDS : IDataStore
    {
        static Dictionary<string, object> _sites;
        string DatabaseName = "CosmoData";
        string CollectionName = "Collection_CosmoData";
        string EndpointUrl = "https://wencocosmodb1.documents.azure.com:443/";
        string PrimaryKey = "ShpsvUeObdx5SXvRKpUSVfnFXkeHJM5R9yJQRG5JK1ZYClJYG66JOdSQkN700Mai51MqBIR2dnod1TKQHHbgFw==";
        DocumentClient documentClient;

        public CosmoDS()
        {
            if (_sites == null)
            {
                _sites = new Dictionary<string, object>();
            }
            documentClient = new DocumentClient(new Uri(EndpointUrl), PrimaryKey);
        }

        public T Get<T>(string key, ref string res)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var site = documentClient.CreateDocumentQuery<Site>(UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName))
                       .Where(r => r.SiteID == Convert.ToInt32(key))
                       .AsEnumerable().FirstOrDefault();            
            stopwatch.Stop();

            res += Environment.NewLine + "Time took to fetch from Cosmo database in milliseconds " + stopwatch.ElapsedMilliseconds;
            return (T)Convert.ChangeType(site, typeof(T));
        }

        public void Put<T>(string key, T instance)
        {
            string site = JsonConvert.SerializeObject(instance);            
            PutAsync(key, instance).Wait();
        }

        public async Task PutAsync<T>(string key, T instance)
        {
            string site = JsonConvert.SerializeObject(instance);
            await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName });
            await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = CollectionName });
            await CreateSiteDocumentIfNotExists(DatabaseName, CollectionName, site);
        }

        private async Task CreateSiteDocumentIfNotExists(string databaseName, string collectionName, string lsite)
        {
            Site _site = JsonConvert.DeserializeObject<Site>(lsite);
            try
            {


                await documentClient.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, _site.SiteID.ToString()));

            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), _site);

                }
                else
                {
                    throw;
                }
            }
        }
    }
}
