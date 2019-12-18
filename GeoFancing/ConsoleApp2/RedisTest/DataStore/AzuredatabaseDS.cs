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
    public class AzuredatabaseDS : IDataStorebyPoint
    {
        AzureDatabaseDtls _azureDatabaseDtls;
        SiteDBLayer _SiteDBLayer;

        public AzuredatabaseDS(AzureDatabaseDtls azureDatabaseDtls, SiteDBLayer SiteDBLayer)
        {
            _azureDatabaseDtls = azureDatabaseDtls;
            _SiteDBLayer = SiteDBLayer;
        }

        public T Get<T>(string key, int X, int Y, ref string res)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var site = _SiteDBLayer.GetSites(Convert.ToInt32(key), X, Y);
            stopwatch.Stop();

            res += Environment.NewLine + "Time took to fetch from Cosmo database in milliseconds " + stopwatch.ElapsedMilliseconds;
            return (T)Convert.ChangeType(site, typeof(T));
        }

        public void Put<T>(string key, T instance)
        {                
            
        }           
    }
}
