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

namespace PerformanceTestLibrary
{
    public class AzuredatabaseDS : IDataStorebyPoint
    {        
        SiteDBLayer _SiteDBLayer;

        public AzuredatabaseDS(SiteDBLayer SiteDBLayer)
        {            
            _SiteDBLayer = SiteDBLayer;
        }

        public T Get<T>(string key, int X, int Y, out long fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var site = _SiteDBLayer.GetSites(Convert.ToInt32(key), X, Y);
            stopwatch.Stop();

            fetchTime = stopwatch.ElapsedMilliseconds;
            return (T)Convert.ChangeType(site, typeof(T));
        }

        public void Put<T>(string key, T instance)
        {                
            
        }           
    }
}
