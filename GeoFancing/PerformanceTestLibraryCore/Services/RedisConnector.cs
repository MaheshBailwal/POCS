using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceTestLibrary
{
    public class RedisConnector
    {
        public RedisConnector(string host)
        {
            this.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(host);
            });
        }

        private Lazy<ConnectionMultiplexer> lazyConnection;

        public IDatabase Connection
        {
            get
            {
                return lazyConnection.Value.GetDatabase();
            }
        }

    }
}
