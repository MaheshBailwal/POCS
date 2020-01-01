using Microsoft.Azure.CosmosDB.BulkExecutor;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PerformanceTestLibrary
{
    public class CosmoDS : IQueryableDataStore
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _endPointUrl;
        private readonly string _primaryKey;
        DocumentClient _documentClient;
        DocumentCollection _documentCollection;
        BatchBlock<string> _buffer = new BatchBlock<string>(100);

        public CosmoDS(string databaseName, string collectionName, string endPointUrl, string primaryKey)
        {
            _databaseName = databaseName;
            _collectionName = collectionName;
            _endPointUrl = endPointUrl;
            _primaryKey = primaryKey;

            _documentClient = new DocumentClient(new Uri(_endPointUrl), _primaryKey, new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            });
            _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName }).Wait();
            _documentCollection = _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseName), new DocumentCollection { Id = _collectionName }).Result;
            var tt = createActionBlock();
            _buffer.LinkTo(tt);
        }

        public T Get<T>(string key, int X, int Y, int width, int height, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // true
            var option = new FeedOptions { EnableCrossPartitionQuery = true };
            var zones = _documentClient.CreateDocumentQuery<Site>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), option)
                       .Where(r => r.SiteID == Convert.ToInt32(key))
                       .AsEnumerable().FirstOrDefault().Zones.
                       Where(r => (r.Rectangle.X >= X && r.Rectangle.X <= X + width)
                       && (r.Rectangle.Y >= Y && r.Rectangle.Y <= Y + height)).ToList();

            stopwatch.Stop();
            fetchTime = stopwatch.Elapsed.TotalMilliseconds;
            return (T)Convert.ChangeType(zones, typeof(T));
        }

        public void Put<T>(string key, T instance)
        {
            try
            {
                lock (_databaseName)
                {
                    PutAsync(key, instance).Wait();
                }
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
                Put(key, instance);
            }
        }

        public async Task PutAsync<T>(string key, T instance)
        {
            await CreateSiteDocumentIfNotExists(_databaseName, _collectionName, instance);
        }

        private async Task CreateSiteDocumentIfNotExists(string databaseName, string collectionName, object site)
        {
            await _documentClient.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), site);
        }

        private async Task CreateSiteDocumentIfNotExistsEx(string databaseName, string collectionName, object site)
        {
            _buffer.Post(JsonConvert.SerializeObject(site));
        }

        private ActionBlock<IEnumerable<string>> createActionBlock()
        {
     
            var block = new ActionBlock<IEnumerable<string>>(i =>
            {
                BulkInsertEx(i).Wait();
            });
            return block;
        }

        private async Task BulkInsertEx(IEnumerable<string> lst)
        {
         
            IBulkExecutor bulkExecutor = new BulkExecutor(_documentClient, _documentCollection);
            await bulkExecutor.InitializeAsync();
            try
            {
                var bulkImportResponse =  bulkExecutor.BulkImportAsync(
                                                   documents: lst,
                                                   enableUpsert: true,
                                                   disableAutomaticIdGeneration: true,
                                                   maxConcurrencyPerPartitionKeyRange: null,
                                                   maxInMemorySortingBatchSize: null,
                                                   cancellationToken: CancellationToken.None).Result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
     
    }
}

