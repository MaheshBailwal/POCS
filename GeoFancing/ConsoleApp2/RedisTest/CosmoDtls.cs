using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class CosmoDtls
    {
        string lDatabaseName = "CosmoData";
        string lCollectionName = "Collection_CosmoData";
        string lEndpointUrl = "https://wencocosmodb1.documents.azure.com:443/";
        string lPrimaryKey = "ShpsvUeObdx5SXvRKpUSVfnFXkeHJM5R9yJQRG5JK1ZYClJYG66JOdSQkN700Mai51MqBIR2dnod1TKQHHbgFw==";

        
        public string DatabaseName
        {
            get => lDatabaseName;
        }
        public string CollectionName
        {
            get => lCollectionName;
        }

        public string EndpointUrl
        {
            get => lEndpointUrl;
        }

        public string PrimaryKey
        {
            get => lPrimaryKey;
        }
    }
}
