using PerformanceTestLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PerformanceTestLibrary
{
    public class SiteDBLayer
    {
        private readonly string _connectionString;

        public SiteDBLayer(string connectionString)
        {            
            _connectionString = connectionString;
        }

        public IEnumerable<Zone> GetZones(int siteID, int X, int Y)
        {            
            int lX =0;
            int lY = 0;
            int lWidth = 0;
            int lHeight = 0;
            //Site site = new Site();
            var zones  = new List<Zone>();
            using (SqlConnection con = new SqlConnection( _connectionString))
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
                    lX= Convert.ToInt32(rdr["X"]);
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
