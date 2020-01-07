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
        public string ToEmails { get; set; }
        public string DoNotPushDataToStores { get; set; }
        public string TestToRun { get; set; }
       
        public string PingToRedisWithPort { get; set; }
        public string PingToCosmoWithPort { get; set; }
        public string PingToSqlWithPort { get; set; }
        public string PingToStorageWithPort { get; set; }  
        public string IsPingNeedToCheck { get; set; }
      
        
    }
}
