using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PerformanceTestLibrary
{
    public class AzureSql : IQueryableDataStore
    {           
        private readonly string _connectionString;

        public AzureSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public T Get<T>(string key, int X, int Y, int width, int height, out double fetchTime)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var site = GetZones(Convert.ToInt32(key), X, Y).ToList();
            
            stopwatch.Stop();

            fetchTime = stopwatch.Elapsed.TotalMilliseconds;
            return (T)Convert.ChangeType(site, typeof(T));
        }

        public void Put<T>(string key, T instance)
        {                
            
        }

        private IEnumerable<Zone> GetZones(int siteID, int X, int Y)
        {
            int lX = 0;
            int lY = 0;
            int lWidth = 0;
            int lHeight = 0;
            //Site site = new Site();
            var zones = new List<Zone>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string strSQL = "select SiteID,X,Y,Width, Height from [Site_Zone] where SiteID=" + siteID + " and (" + X + ">=X and " +
                    X + "<=(X + Width)) and (" + Y + ">=Y and " + Y + "<= (Y + Height))";
                SqlCommand cmd = new SqlCommand(strSQL, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                //site = new Site();
                while (rdr.Read())
                {
                    //site.SiteID = Convert.ToInt32(rdr["SiteID"]);                    
                    lX = Convert.ToInt32(rdr["X"]);
                    lY = Convert.ToInt32(rdr["Y"]);
                    lWidth = Convert.ToInt32(rdr["Width"]);
                    lHeight = Convert.ToInt32(rdr["Height"]);
                    zones.Add(new Zone() { Rectangle = new System.Drawing.Rectangle() { X = lX, Y = lY, Width = lWidth, Height = lHeight }, PolyGon = null });

                    //lstSite.Add(site);
                }
                con.Close();
            }
            return zones;
        }
    }
}
