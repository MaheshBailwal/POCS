using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class AppSettings
    {
        public string RedisCacheConfig { get; set; }
        public string CosmoDatabaseName { get; set; }
        public string CosmoCollectionName { get; set; }
        public string CosmoEndpointUrl { get; set; }
        public string CosmoPrimaryKey { get; set; }
        public string AzureDBConnectionString { get; set; }
        public string StorageConnectionstring { get; set; }
        public string ContainerName { get; set; }
        public string NumberOfSites { get; set; }
        public string NumberOfZones { get; set; }
        public string NumberOfIteration { get; set; }
    }
}
