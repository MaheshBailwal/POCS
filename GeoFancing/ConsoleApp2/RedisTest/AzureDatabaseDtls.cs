using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class AzureDatabaseDtls
    {
        string lConnectionString = "Server=tcp:wencodbserver.database.windows.net,1433;Initial Catalog=wencodb;Persist Security Info=False;User ID=wencoadmin;Password=P@ssword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public string ConnectionString
        {
            get => lConnectionString;
        }

    }
}
