using ConsoleApp2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RedisTest
{
    public class AzureDatabaseAccessLayer
    {       

        public Site GetSites(int siteID, int X, int Y)
        {
            List<Site> lstSite = new List<Site>();
            int lX =0;
            int lY = 0;
            int lWidth = 0;
            int lHeight = 0;
            Site site = new Site();
            site.Zones = new List<Zone>();
            using (SqlConnection con = new SqlConnection(Util.ConnectionString.CName))
            {
                SqlCommand cmd = new SqlCommand("sp_sites_get", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@siteid", SqlDbType.Int);
                cmd.Parameters.Add("@x", SqlDbType.Int);
                cmd.Parameters.Add("@y", SqlDbType.Int);
                cmd.Parameters["@siteid"].Value = siteID;
                cmd.Parameters["@x"].Value = X;
                cmd.Parameters["@y"].Value = Y;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                //site = new Site();
                while (rdr.Read())
                {
                    
                    site.SiteID = Convert.ToInt32(rdr["SiteID"]);                    
                    lX= Convert.ToInt32(rdr["X"]);
                    lY = Convert.ToInt32(rdr["Y"]);
                    lWidth = Convert.ToInt32(rdr["Width"]);
                    lHeight = Convert.ToInt32(rdr["Height"]);
                    site.Zones.Add(new Zone() { Rectangle = new System.Drawing.Rectangle() { X = lX, Y = lY, Width = lWidth, Height = lHeight }, PolyGon = null });
                    
                    //lstSite.Add(site);
                }
                con.Close();
            }
            return site;
        }

    }
}
