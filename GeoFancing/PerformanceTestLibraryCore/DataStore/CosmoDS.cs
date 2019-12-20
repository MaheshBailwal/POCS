using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PerformanceTestLibrary
{
    public class CosmoDS : IQueryableDataStore
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _endPointUrl;
        private readonly string _primaryKey;
        DocumentClient documentClient;        

        public CosmoDS(string databaseName, string collectionName, string endPointUrl, string primaryKey)
        {            
            _databaseName = databaseName;
            _collectionName = collectionName;
            _endPointUrl = endPointUrl;
            _primaryKey = primaryKey;
            documentClient = new DocumentClient(new Uri(_endPointUrl), _primaryKey);
        }

        public T Get<T>(string key,int X, int Y, int width, int height, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var site = documentClient.CreateDocumentQuery<Site>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName))
                       .Where(r => r.SiteID == Convert.ToInt32(key))
                       .AsEnumerable().FirstOrDefault().Zones.Where(r => (r.Rectangle.X >= X && r.Rectangle.X <= X+width) && (r.Rectangle.Y >= Y && r.Rectangle.X <=Y+height));
            stopwatch.Stop();
            fetchTime = stopwatch.Elapsed.TotalMilliseconds;            
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
            await documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName });
            await documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseName), new DocumentCollection { Id = _collectionName });
            await CreateSiteDocumentIfNotExists(_databaseName, _collectionName, site);
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
